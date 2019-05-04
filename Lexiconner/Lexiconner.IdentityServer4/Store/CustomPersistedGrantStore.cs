using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Lexiconner.Persistence.Repositories.Base;

namespace Lexiconner.IdentityServer4.Store
{
    /// <summary>
    /// Handle consent decisions, authorization codes, refresh and reference tokens
    /// </summary>
    public class CustomPersistedGrantStore : IPersistedGrantStore
    {
        protected IMongoRepository _dbRepository;

        public CustomPersistedGrantStore(IMongoRepository repository)
        {
            _dbRepository = repository;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = await _dbRepository.GetManyAsync<PersistedGrant>(i => i.SubjectId == subjectId);
            return result;
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var result = await _dbRepository.GetOneAsync<PersistedGrant>(i => i.Key == key);
            return result;
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _dbRepository.DeleteAsync<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _dbRepository.DeleteAsync<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId && i.Type == type);
        }

        public async Task RemoveAsync(string key)
        {
            await _dbRepository.DeleteAsync<PersistedGrant>(i => i.Key == key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await _dbRepository.AddAsync<PersistedGrant>(grant);
        }
    }
}
