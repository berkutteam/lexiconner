using IdentityServer4.Models;
using Lexiconner.Domain.Entitites.Base;
using Lexiconner.Persistence.Repositories.Base;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Lexiconner.Persistence.Repositories.MongoDb
{
    /// <summary>
    /// Provides functionality  to persist "IdentityServer4.Models" into a given MongoDB
    /// </summary>
    public class MongoRepository : IMongoRepository
    {
        protected readonly string _databseName;
        protected readonly IMongoClient _client;
        protected readonly IMongoDatabase _database;

        private const int _maxPageSize = 1000;

        public MongoRepository(MongoClient client, string database)
        {
            _databseName = database;
            _client = client;
            _database = client.GetDatabase(database); // db will be created if not exists
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public Task DropDatabaseAsync()
        {
            return _client.DropDatabaseAsync(_databseName);
        }

        public async Task<bool> CollectionExistsAsync<T>() where T : class, new()
        {
            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>());
            var filter = new BsonDocument();
            var totalCount = await collection.CountDocumentsAsync(filter);
            return totalCount != 0;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).Find(x => true);
            List<T> result = await query.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int offset, int limit, string search = "") where T : class, new()
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).Find(x => true);
            var result = await query.Skip(offset)
                .Limit(limit > _maxPageSize ? _maxPageSize : limit)
                .ToListAsync();
            return result;
        }

        public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var cursor = await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).FindAsync(predicate);
            var result = await cursor.FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).Find(predicate);
            List<T> result = await query.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate, int offset, int limit, string search = "") where T : class, new()
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).Find(predicate);
            var result = await query.Skip(offset)
                .Limit(limit > _maxPageSize ? _maxPageSize : limit)
                .ToListAsync();
            return result;
        }

        public async Task AddAsync<T>(T entity) where T : class, new()
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).InsertOneAsync(entity);
        }

        public async Task AddAsync<T>(IEnumerable<T> entities) where T : class, new()
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).InsertManyAsync(entities);
        }

        // TODO - check works properly
        public async Task UpdateAsync<T>(T entity) where T : BaseEntity, new()
        {
            var definition = new ObjectUpdateDefinition<T>(entity);
            var result = await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).UpdateOneAsync(x => x.Id == entity.Id, definition);
            if (!result.IsAcknowledged)
            {
                throw new Exception($"{nameof(UpdateAsync)} wasn't acknowledged!");
            }
        }

        // TODO - check works properly
        public async Task UpdateAsync<T>(IEnumerable<T> entities) where T : BaseEntity, new()
        {
            var ids = entities.Select(x => x.Id).ToList();
            var updates = new List<WriteModel<T>>();
            var filterBuilder = Builders<T>.Filter;
            foreach (var entity in entities)
            {
                var filter = filterBuilder.Where(x => x.Id == entity.Id);
                updates.Add(new ReplaceOneModel<T>(filter, entity));
            }
            var result = await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).BulkWriteAsync(updates);
            if (!result.IsAcknowledged)
            {
                throw new Exception($"{nameof(UpdateAsync)} wasn't acknowledged!");
            }
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).DeleteManyAsync(predicate);
        }

        public async Task DeleteAllAsync<T>() where T : class, new()
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).DeleteManyAsync(x => true);
        }

        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>());
            var cursor = await collection.FindAsync(predicate);
            return await cursor.AnyAsync();
        }

        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>());
            var cursor = await collection.FindAsync(predicate);
            return await cursor.AnyAsync();
        }

        public async Task<long> CountAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).Find(predicate);
            return await query.CountDocumentsAsync();
        }
    }
}
