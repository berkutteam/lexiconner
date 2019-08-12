using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lexiconner.Domain.Config;
using MongoDB.Driver;

namespace Lexiconner.Persistence.Repositories.MongoDb
{
    /// <summary>
    /// Basic interface with a few methods for adding, deleting, and querying data.
    /// </summary>
    public interface IMongoDataRepository : IDataRepository
    {
        /// <summary>
        /// Returns Mongo database
        /// </summary>
        /// <returns></returns>
        IMongoDatabase GetDatabase();

        /// <summary>
        /// Drops database
        /// </summary>
        /// <returns></returns>
        Task DropDatabaseAsync();

        Task InitializeCollectionAsync<T>();
       // Task InitializeCollections(List<MongoCollectionConfig> mongoCollectionConfigs);
    }
}
