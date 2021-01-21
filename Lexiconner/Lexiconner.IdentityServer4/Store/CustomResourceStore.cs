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
    public class CustomResourceStore : IResourceStore
    {
        protected IDataRepository _dataRepository;

        public CustomResourceStore(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        private async Task<IEnumerable<ApiResource>> GetAllApiResources()
        {
            var result = await _dataRepository.GetAllAsync<ApiResourceEntity>();
            return result.Select(x => x.ApiResource);
        }

        private async Task<IEnumerable<ApiScope>> GetAllApiScopes()
        {
            var result = await _dataRepository.GetAllAsync<ApiScopeEntity>();
            return result.Select(x => x.ApiScope);
        }

        private async Task<IEnumerable<IdentityResource>> GetAllIdentityResources()
        {
            var result = await _dataRepository.GetAllAsync<IdentityResourceEntity>();
            return result.Select(x => x.IdentityResource);
        }

        //public async Task<ApiResource> FindApiResourceAsync(string name)
        //{
        //    if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        //    var result = await _dataRepository.GetOneAsync<ApiResourceEntity>(a => a.ApiResource.Name == name);
        //    return result.ApiResource;
        //}

        //public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        //{
        //    var result = await _dataRepository.GetManyAsync<ApiResourceEntity>(a => a.ApiResource.Scopes.Any(s => scopeNames.Contains(s.Name)));
        //    return result.Select(x => x.ApiResource);
        //}

        //public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        //{
        //    var result = await _dataRepository.GetManyAsync<IdentityResourceEntity>(e => scopeNames.Contains(e.IdentityResource.Name));
        //    return result.Select(x => x.IdentityResource);
        //}


        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var result = await _dataRepository.GetManyAsync<ApiResourceEntity>(x => apiResourceNames.Contains(x.ApiResource.Name));
            return result.Select(x => x.ApiResource);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var all = await _dataRepository.GetAllAsync<ApiResourceEntity>();
            var result = all.Where(x => x.ApiResource.Scopes.Any(scope => scopeNames.Contains(scope))).ToList();
            return result.Select(x => x.ApiResource);
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var result = await _dataRepository.GetManyAsync<ApiScopeEntity>(x => scopeNames.Contains(x.ApiScope.Name));
            return result.Select(x => x.ApiScope);
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var result = await _dataRepository.GetManyAsync<IdentityResourceEntity>(x => scopeNames.Contains(x.IdentityResource.Name));
            return result.Select(x => x.IdentityResource);
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(
                await GetAllIdentityResources(),
                await GetAllApiResources(),
                await GetAllApiScopes()
            );
            return result;
        }
    }
}
