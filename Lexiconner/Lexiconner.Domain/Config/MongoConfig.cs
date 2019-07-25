using IdentityServer4.Models;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Config
{
    public static class MongoConfig
    {
        public const string IdentityUsers = "identityUsers";
        public const string IdentityRoles = "identityRoles";

        public const string IdentityApiResources = "identityApiResources";
        public const string IdentityClients = "identityClients";
        public const string IdentityIdentityResources = "identityIdentityResources";
        public const string IdentityPersistedGrant = "identityPersistedGrant";

        public const string StudyItems = "studyItems";

        /// <summary>
        /// Defines list of allowed collections in database
        /// </summary>
        private static Dictionary<string, string> _collectionNameMap = new Dictionary<string, string>
        {
            // Identity
            { typeof(ApplicationUserEntity).Name,  IdentityUsers },
            { typeof(ApplicationRoleEntity).Name,  IdentityRoles },

            // Identity Server
            { typeof(ApiResource).Name,      IdentityApiResources },
            { typeof(Client).Name,           IdentityClients },
            { typeof(IdentityResource).Name, IdentityIdentityResources },
            { typeof(PersistedGrant).Name,   IdentityPersistedGrant },

            // custom entities
            { typeof(StudyItemEntity).Name,   StudyItems },
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
