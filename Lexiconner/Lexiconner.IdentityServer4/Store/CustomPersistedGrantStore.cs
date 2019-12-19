using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Lexiconner.Domain.Entitites.IdentityModel;
using Lexiconner.Persistence.Repositories;

namespace Lexiconner.IdentityServer4.Store
{
    /// <summary>
    /// Handle consent decisions, authorization codes, refresh and reference tokens
    /// </summary>
    public class CustomPersistedGrantStore : IPersistedGrantStore
    {
        protected IDataRepository _dataRepository;

        public CustomPersistedGrantStore(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = await _dataRepository.GetManyAsync<PersistedGrantEntity>(i => i.PersistedGrant.SubjectId == subjectId);
            return result.Select(x => x.PersistedGrant);
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var result = await _dataRepository.GetOneAsync<PersistedGrantEntity>(i => i.PersistedGrant.Key == key);
            return result.PersistedGrant;
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _dataRepository.DeleteAsync<PersistedGrantEntity>(i => i.PersistedGrant.SubjectId == subjectId && i.PersistedGrant.ClientId == clientId);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _dataRepository.DeleteAsync<PersistedGrantEntity>(i => i.PersistedGrant.SubjectId == subjectId && i.PersistedGrant.ClientId == clientId && i.PersistedGrant.Type == type);
        }

        public async Task RemoveAsync(string key)
        {
            await _dataRepository.DeleteAsync<PersistedGrantEntity>(i => i.PersistedGrant.Key == key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await _dataRepository.AddAsync<PersistedGrantEntity>(new PersistedGrantEntity(grant));
        }
    }
}
