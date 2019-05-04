using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Lexiconner.IdentityServer4.Repository;

namespace Lexiconner.IdentityServer4.Store
{
    public class CustomClientStore : IClientStore
    {
        protected IMongoRepository _dbRepository;

        public CustomClientStore(IMongoRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return Task.FromResult(client);
        }
    }
}
