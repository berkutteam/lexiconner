using IdentityServer4.Models;
using Lexiconner.IdentityServer4.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4.Repository
{
    public static class MongoConfig
    {
        /// <summary>
        /// Defines list of allowed collections in database
        /// </summary>
        private static Dictionary<string, string> _collecationNameMap = new Dictionary<string, string>
        {
            { typeof(ApplicationUser).Name,  "identityUsers" },
            { typeof(ApplicationRole).Name,  "identityRoles" },
            { typeof(ApiResource).Name,      "identityApiResources" },
            { typeof(Client).Name,           "identityClients" },
            { typeof(IdentityResource).Name, "identityIdentityResources" },
            { typeof(PersistedGrant).Name,   "identityPersistedGrant" },
        };

        public static string GetCollectionName<T>()
        {
            if(_collecationNameMap.ContainsKey(typeof(T).Name))
            {
                return _collecationNameMap[typeof(T).Name];
            }

            throw new InvalidOperationException($"{typeof(T).Name} is not registered in collection list!");
        }
    }
}
