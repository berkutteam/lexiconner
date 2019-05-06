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
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new();
        Task<IEnumerable<T>> GetAllAsync<T>(int offset, int limit, string search = "") where T : class, new();
        Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate, int offset, int limit, string search = "") where T : class, new();
        Task<T> GetOneAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        Task AddAsync<T>(T entity) where T : class, new();
        Task AddAsync<T>(IEnumerable<T> entities) where T : class, new();
        Task UpdateAsync<T>(T entity) where T : BaseEntity, new();
        Task UpdateAsync<T>(IEnumerable<T> entities) where T : BaseEntity, new();
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        Task DeleteAllAsync<T>() where T : class, new();
        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        Task<long> CountAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
    }
}
