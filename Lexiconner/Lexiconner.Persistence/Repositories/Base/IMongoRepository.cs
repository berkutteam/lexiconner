using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Lexiconner.Persistence.Repositories.Base
{
    /// <summary>
    /// Basic interface with a few methods for adding, deleting, and querying data.
    /// </summary>
    public interface IMongoRepository : IRepositoryBase
    {
        /// <summary>
        /// Returns Mngo database
        /// </summary>
        /// <returns></returns>
        IMongoDatabase GetDatabase();

        /// <summary>
        /// Checks that collection exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<bool> CollectionExistsAsync<T>() where T : class, new();
    }
}
