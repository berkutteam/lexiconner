using Lexiconner.Domain.Entitites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.Repositories.Base
{
    public interface IRepositoryBase
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>(int offset, int limit) where T : class;
        Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate, int offset, int limit) where T : class;
        Task<T> GetOneAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task AddAsync<T>(T entity) where T : class;
        Task AddManyAsync<T>(IEnumerable<T> entities) where T : class;
        Task UpdateAsync<T>(T entity) where T : BaseEntity;
        Task UpdateManyAsync<T>(IEnumerable<T> entities) where T : BaseEntity;
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task DeleteAllAsync<T>() where T : class;

        /// <summary>
        /// Deletes only N documents by predicate selected after order
        /// </summary>
        /// <returns></returns>
        Task DeleteNDocumentsAsync<T>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> sortFieldSelector,
            int deleteCount
        ) where T : class, IIdentifiableEntity;

        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<long> CountAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    }
}
