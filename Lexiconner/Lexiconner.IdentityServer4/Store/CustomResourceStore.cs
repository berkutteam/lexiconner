using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Lexiconner.Persistence.Repositories;

namespace Lexiconner.IdentityServer4.Store
{
    public class CustomResourceStore : IResourceStore
    {
        protected IDataRepository _dataRepository;

        public CustomResourceStore(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        private async Task<IEnumerable<ApiResource>> GetAllApiResources()
        {
            return await _dataRepository.GetAllAsync<ApiResource>();
        }

        private async Task<IEnumerable<IdentityResource>> GetAllIdentityResources()
        {
            return await _dataRepository.GetAllAsync<IdentityResource>();
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return await _dataRepository.GetOneAsync<ApiResource>(a => a.Name == name);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return await _dataRepository.GetManyAsync<ApiResource>(a => a.Scopes.Any(s => scopeNames.Contains(s.Name)));
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return await _dataRepository.GetManyAsync<IdentityResource>(e => scopeNames.Contains(e.Name));
        }

        public async Task<Resources> GetAllResources()
        {
            var result = new Resources(await GetAllIdentityResources(), await GetAllApiResources());
            return result;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            return await GetAllResources();
        }
    }
}
