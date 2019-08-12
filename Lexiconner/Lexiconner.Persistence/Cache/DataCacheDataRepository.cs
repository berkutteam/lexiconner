using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.Cache
{
    public class DataCacheDataRepository : IDataCache
    {
        private readonly int _documentsMaxLimit = 50_000;
        private readonly int _documentsToDeleteOnMaxLimit = 1000;

        private readonly ILogger<IDataCache> _logger;
        private readonly IDataRepository _dataRepository;

        public DataCacheDataRepository(
            ILogger<IDataCache> logger,
            IDataRepository dataRepository,
            int documentsMaxLimit = 50_000,
            int documentsToDeleteOnMaxLimit = 1000
        )
        {
            _logger = logger;
            _dataRepository = dataRepository;

            _documentsMaxLimit = documentsMaxLimit <= 10_000_000 ? documentsMaxLimit : 10_000_000;
            _documentsToDeleteOnMaxLimit = documentsToDeleteOnMaxLimit;
        }

        public async Task<T> Get<T>(Expression<Func<T, bool>> predicate) where T : DataCacheBaseEntity
        {
            return await _dataRepository.GetOneAsync<T>(predicate);
        }

        public async Task Add<T>(T entity) where T : DataCacheBaseEntity
        {
            await _dataRepository.AddAsync<T>(entity);
            await HandleLimits<T>();
        }

        public async Task Delete<T>(Expression<Func<T, bool>> predicate) where T : DataCacheBaseEntity
        {
            await _dataRepository.DeleteAsync<T>(predicate);
        }

        public async Task Clear<T>() where T : DataCacheBaseEntity
        {
            await _dataRepository.DeleteAllAsync<T>();
        }

        private async Task HandleLimits<T>() where T : DataCacheBaseEntity
        {
            long count = await _dataRepository.CountAllAsync<T>(x => true);
            if(count > _documentsMaxLimit)
            {
                _logger.LogInformation($"Cache limit of {_documentsMaxLimit} documents is exceeded. Delete {_documentsToDeleteOnMaxLimit} documents to free up the space.");
                await _dataRepository.DeleteNDocumentsAsync<T>(x => true, x => x.CreatedAt, deleteCount: _documentsToDeleteOnMaxLimit);
            }
        }
    }
}
