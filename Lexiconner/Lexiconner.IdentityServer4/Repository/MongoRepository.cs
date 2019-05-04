//using IdentityServer4.Models;
//using Microsoft.Extensions.Options;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;


//namespace Lexiconner.IdentityServer4.Repository
//{
//    /// <summary>
//    /// Provides functionality  to persist "IdentityServer4.Models" into a given MongoDB
//    /// </summary>
//    public class MongoRepository : IMongoRepository
//    {
//        protected readonly IMongoClient _client;
//        protected readonly IMongoDatabase _database;

//        /// <summary>
//        /// This Contructor leverages  .NET Core built-in DI
//        /// </summary>
//        /// <param name="optionsAccessor">Injected by .NET Core built-in Depedency Injection</param>
//        public MongoRepository(IOptions<ApplicationSettings> config)
//        {
//            var configurationOptions = config.Value;

//            _client = new MongoClient(configurationOptions.MongoDb.ConnectionString);
//            _database = _client.GetDatabase(configurationOptions.MongoDb.Database);

//        }

//        /// <summary>
//        /// Get Database connection
//        /// </summary>
//        /// <returns></returns>
//        public IMongoDatabase GetDatabase()
//        {
//            return _database;
//        }

//        public IQueryable<T> All<T>() where T : class, new()
//        {
//            return _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).AsQueryable();
//        }

//        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
//        {
//            return All<T>().Where(expression);
//        }

//        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
//        {
//            return All<T>().Where(expression).SingleOrDefault();
//        }

//        public bool CollectionExists<T>() where T : class, new()
//        {
//            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>());
//            var filter = new BsonDocument();
//            var totalCount = collection.CountDocuments(filter);
//            return totalCount != 0;

//        }

//        public bool Exists<T>(Expression<Func<T, bool>> expression) where T : class, new()
//        {
//            var collection = _database.GetCollection<T>(MongoConfig.GetCollectionName<T>());
//            var exixting = collection.FindAsync(expression).GetAwaiter().GetResult();
//            return exixting.Current != null;
//        }

//        public void Add<T>(T item) where T : class, new()
//        {
//            _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).InsertOne(item);
//        }

//        public void Add<T>(IEnumerable<T> items) where T : class, new()
//        {
//            _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).InsertMany(items);
//        }

//        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
//        {
//            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).DeleteManyAsync(predicate);
//        }

//        public async Task DeleteAllAsync<T>() where T : class, new()
//        {
//            await _database.GetCollection<T>(MongoConfig.GetCollectionName<T>()).DeleteManyAsync(x => true);
//        }
//    }
//}
