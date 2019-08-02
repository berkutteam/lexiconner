using IdentityServer4.Models;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites.Base;
using Lexiconner.Domain.Enums;
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
        protected readonly ApplicationDb _applicationDb;
        protected readonly IMongoClient _client;
        protected readonly IMongoDatabase _database;

        private const int _maxPageSize = 1000;

        public MongoRepository(MongoClient client, string database, ApplicationDb applicationDb)
        {
            _databseName = database;
            _applicationDb = applicationDb;
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

        public async Task<bool> CollectionExistsAsync<T>() where T : class
        {
            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb));
            var filter = new BsonDocument();
            var totalCount = await collection.CountDocumentsAsync(filter);
            return totalCount != 0;
        }

        public async Task InitializeCollection<T>()
        {
            MongoCollectionConfig mongoCollectionConfig = MongoConfig.GetCollectionConfig<T>(_applicationDb);

            // create
            var collections = await (await _database.ListCollectionNamesAsync()).ToListAsync();
            if(!collections.Contains(mongoCollectionConfig.CollectionName))
            {
                await _database.CreateCollectionAsync(mongoCollectionConfig.CollectionName);
            }
            var collection = _database.GetCollection<T>(mongoCollectionConfig.CollectionName);
            foreach (var index in mongoCollectionConfig.Indexes)
            {
                var indexOptions = new CreateIndexOptions();
                var indexKeys = $"{{ {index}: 1 }}"; // { <field>: <order: 1:asc, -1:desc > }
                var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);
                await collection.Indexes.CreateOneAsync(indexModel);
            }
        }

        //// TODO
        //public async Task InitializeCollections(List<MongoCollectionConfig> mongoCollectionConfigs)
        //{
        //    foreach (var collectionConfig in mongoCollectionConfigs)
        //    {
        //        // create
        //        await _database.CreateCollectionAsync(collectionConfig.CollectionName);

        //        // call IMongoDatabase.GetCollection<T> generic method using runtime collection type
        //        var collectionObj = typeof(IMongoDatabase)
        //            .GetMethod(nameof(IMongoDatabase.GetCollection))
        //            .MakeGenericMethod(collectionConfig.CollectionType)
        //            .Invoke(_database, new object[] { collectionConfig.CollectionName });
        //        var collectionType
        //        var collection = collectionObj as IMongoCollection<>;

        //        // TODO add indexes
        //        // https://stackoverflow.com/questions/17807577/how-to-create-indexes-in-mongodb-via-net

        //    }
        //}

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).Find(x => true);
            List<T> result = await query.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int offset, int limit, string search = "") where T : class
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).Find(x => true);
            var result = await query.Skip(offset)
                .Limit(limit > _maxPageSize ? _maxPageSize : limit)
                .ToListAsync();
            return result;
        }

        public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var cursor = await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).FindAsync(predicate);
            var result = await cursor.FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).Find(predicate);
            List<T> result = await query.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate, int offset, int limit, string search = "") where T : class
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).Find(predicate);
            var result = await query.Skip(offset)
                .Limit(limit > _maxPageSize ? _maxPageSize : limit)
                .ToListAsync();
            return result;
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).InsertOneAsync(entity);
        }

        public async Task AddManyAsync<T>(IEnumerable<T> entities) where T : class
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).InsertManyAsync(entities);
        }

        public async Task UpdateAsync<T>(T entity) where T : BaseEntity
        {
            ///// FYI - update throws exception 'MongoDB Element name _id not valid'.
            /// This happerns when model has Id property that mongo driver maps to _id and when update you can't change it
            // var definition = new ObjectUpdateDefinition<T>(entity);
            //var result = await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).UpdateOneAsync(x => x.Id == model.Id, definition, new UpdateOptions {
            //    IsUpsert = false, // upsert requires _id. otherwise exception will be thrown
            //});

            var result = await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).ReplaceOneAsync(x => x.Id == entity.Id, entity);
            if (!result.IsAcknowledged)
            {
                throw new Exception($"{nameof(UpdateAsync)} wasn't acknowledged!");
            }
        }

        // TODO - check works properly
        public async Task UpdateAsync<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            var ids = entities.Select(x => x.Id).ToList();
            var updates = new List<WriteModel<T>>();
            var filterBuilder = Builders<T>.Filter;
            foreach (var entity in entities)
            {
                var filter = filterBuilder.Where(x => x.Id == entity.Id);
                updates.Add(new ReplaceOneModel<T>(filter, entity));
            }
            var result = await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).BulkWriteAsync(updates);
            if (!result.IsAcknowledged)
            {
                throw new Exception($"{nameof(UpdateAsync)} wasn't acknowledged!");
            }

        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).DeleteManyAsync(predicate);
        }

        public async Task DeleteAllAsync<T>() where T : class
        {
            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).DeleteManyAsync(x => true);
        }

        public async Task DeleteNDcoumentsAsync<T>(
            Expression<Func<T, bool>> predicate, 
            Expression<Func<T, object>> sortFieldSelector, 
            int deleteCount
        ) where T : class, IIdentifiableEntity
        {
            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb));

            var query = collection.Find(predicate);
            var result = await query.SortByDescending(sortFieldSelector)
                .Limit(deleteCount)
                .Project(x => new { Id = x.Id })
                .ToListAsync();
            IEnumerable<string> ids = result.Select(x => x.ToString()).ToList();

            await collection.DeleteManyAsync(x => ids.Contains(x.Id));
        }

        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb));
            var cursor = await collection.FindAsync(predicate);
            return await cursor.AnyAsync();
        }

        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb));
            var cursor = await collection.FindAsync(predicate);
            return await cursor.AnyAsync();
        }

        public async Task<long> CountAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var query = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>(_applicationDb)).Find(predicate);
            return await query.CountDocumentsAsync();
        }
    }
}
