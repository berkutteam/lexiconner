using Lexiconner.Domain.Entitites.Cache;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.Cache
{
    public interface IDataCache
    {
        Task<T> Get<T>(Expression<Func<T, bool>> predicate) where T : DataCacheBaseEntity;
        Task Add<T>(T entity) where T : DataCacheBaseEntity;
        Task Delete<T>(Expression<Func<T, bool>> predicate) where T : DataCacheBaseEntity;
        Task Clear<T>() where T : DataCacheBaseEntity;
    }
}
