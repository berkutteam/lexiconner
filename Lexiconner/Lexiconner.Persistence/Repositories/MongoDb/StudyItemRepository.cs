using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexiconner.Domain.Entitites;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Lexiconner.Persistence.Repositories.MongoDb
{
    public class StudyItemRepository : IStudyItemRepository
    {
        private const string _mongoCollectionName = "studyItems";
        private const int _maxPageSize = 1000;

        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<StudyItem> _mongoCollection;

        public StudyItemRepository(MongoClient mongoClient, string database)
        {
            _mongoClient = mongoClient;
            _mongoDatabase = mongoClient.GetDatabase(database); // db will be created if not exists
            _mongoCollection = _mongoDatabase.GetCollection<StudyItem>(_mongoCollectionName);
        }

        #region Implementation

        public async Task Add(StudyItem entity)
        {
            await _mongoCollection.InsertOneAsync(entity);
        }

        public async Task AddAll(IEnumerable<StudyItem> entities)
        {
            await _mongoCollection.InsertManyAsync(entities);
        }

        public async Task<long> CountAll()
        {
            var query = _mongoCollection.Find(x => true);
            return await query.CountAsync();
        }

        public async Task Delete(StudyItem entity)
        {
            await _mongoCollection.DeleteOneAsync(x => x.Id == entity.Id);
        }

        public async Task Delete(IEnumerable<StudyItem> entities)
        {
            var ids = entities.Select(x => x.Id).ToList();
            await _mongoCollection.DeleteManyAsync(x => ids.Contains(x.Id));
        }

        public async Task Delete(Func<StudyItem, bool> predicate)
        {
            await _mongoCollection.DeleteOneAsync(x => predicate(x));
        }

        public async Task<bool> Exists(Func<StudyItem, bool> predicate)
        {
            var cursor = await _mongoCollection.FindAsync(x => predicate(x));
            var result = await cursor.AnyAsync();
            return result;
        }

        public async Task<StudyItem> Get(Func<StudyItem, bool> predicate)
        {
            var cursor = await _mongoCollection.FindAsync(x => predicate(x));
            var result = await cursor.FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<StudyItem>> GetAll()
        {
            var cursor = await _mongoCollection.FindAsync(x => true);
            var result = await cursor.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<StudyItem>> GetAll(int offset = 0, int limit = 10, string search = "")
        {
            var query = _mongoCollection.Find(x => true);
            var result = await query.Skip(offset)
                .Limit(limit > _maxPageSize ? _maxPageSize : limit)
                .ToListAsync();
            return result;
        }

        public async Task<StudyItem> GetById(string id)
        {
            var cursor = await _mongoCollection.FindAsync(x => x.Id == id);
            var result = await cursor.FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<StudyItem>> GetMany(Func<StudyItem, bool> predicate)
        {
            var cursor = await _mongoCollection.FindAsync(x => predicate(x));
            var result = await cursor.ToListAsync();
            return result;
        }

        public async Task Update(StudyItem entity)
        {
            var definition = new ObjectUpdateDefinition<StudyItem>(entity);
            var result = await _mongoCollection.UpdateOneAsync(x => x.Id == entity.Id, definition);
            if(!result.IsAcknowledged)
            {
                throw new Exception($"{nameof(Update)} wasn't acknowledged!");
            }
        }

        public async Task UpdateAll(IEnumerable<StudyItem> entities)
        {
            var ids = entities.Select(x => x.Id).ToList();
            var updates = new List<WriteModel<StudyItem>>();
            var filterBuilder = Builders<StudyItem>.Filter;
            foreach (var entity in entities)
            {
                var filter = filterBuilder.Where(x => x.Id == entity.Id);
                updates.Add(new ReplaceOneModel<StudyItem>(filter, entity));
            }
            var result = await _mongoCollection.BulkWriteAsync(updates);
            if (!result.IsAcknowledged)
            {
                throw new Exception($"{nameof(UpdateAll)} wasn't acknowledged!");
            }
        }

        #endregion
    }
}
