using IdentityServer4.Models;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Persistence.Repositories.MongoDb
{
    public static class MongoConfig
    {
        /// <summary>
        /// Defines list of allowed collections in database
        /// </summary>
        private static Dictionary<string, string> _collectionNameMap = new Dictionary<string, string>
        {
            // Identity
            { typeof(ApplicationUserEntity).Name,  "identityUsers" },
            { typeof(ApplicationRoleEntity).Name,  "identityRoles" },

            // Identity Server
            { typeof(ApiResource).Name,      "identityApiResources" },
            { typeof(Client).Name,           "identityClients" },
            { typeof(IdentityResource).Name, "identityIdentityResources" },
            { typeof(PersistedGrant).Name,   "identityPersistedGrant" },

            // custom entities
            { typeof(StudyItemEntity).Name,   "studyItems" },
        };

        public static string GetCollectionName<T>()
        {
            if(_collectionNameMap.ContainsKey(typeof(T).Name))
            {
                return _collectionNameMap[typeof(T).Name];
            }

            throw new InvalidOperationException($"{typeof(T).Name} is not registered in collection list!");
        }
    }
}
