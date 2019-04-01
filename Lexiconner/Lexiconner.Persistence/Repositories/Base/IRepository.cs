using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.Repositories.Base
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get all entities of type T
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// Get entities using predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetMany(Func<T, bool> predicate);

        /// <summary>
        /// Get entity by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> Get(Func<T, bool> predicate);

        /// <summary>
        /// Get Entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetById(string id);

        /// <summary>
        /// Marks entity as new
        /// </summary>
        /// <param name="entity"></param>
        Task Add(T entity);

        /// <summary>
        /// Marks all entities as new
        /// </summary>
        /// <param name="entities"></param>
        Task AddAll(IEnumerable<T> entities);

        /// <summary>
        /// Marks entity as modified
        /// </summary>
        /// <param name="entity"></param>
        Task Update(T entity);

        /// <summary>
        /// Marks all entities as modified
        /// </summary>
        /// <param name="entities"></param>
        Task UpdateAll(IEnumerable<T> entities);

        /// <summary>
        /// Marks entity to be removed
        /// </summary>
        /// <param name="entity"></param>
        Task Delete(T entity);

        /// <summary>
        /// Marks entities to be removed
        /// </summary>
        /// <param name="entity"></param>
        Task Delete(IEnumerable<T> entities);

        /// <summary>
        /// Marks entity to be removed accordingly to predicate
        /// </summary>
        /// <param name="predicate"></param>
        Task Delete(Func<T, bool> predicate);

        /// <summary>
        /// Checks entity exists based on predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> Exists(Func<T, bool> predicate);

        /// <summary>
        /// Returns total count of entities in collection
        /// </summary>
        /// <returns></returns>
        Task<long> CountAll();
    }
}
