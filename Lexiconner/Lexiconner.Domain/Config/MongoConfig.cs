using IdentityServer4.Models;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Enums;
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

        private const string IdentityApiResources = "identityApiResources";
        private const string IdentityClients = "identityClients";
        private const string IdentityIdentityResources = "identityIdentityResources";
        private const string IdentityPersistedGrant = "identityPersistedGrant";

        private const string StudyItems = "studyItems";

        private const string CacheGoogleTranslateApi = "cacheGoogleTranslateApi";
        private const string CacheContextualWebSearchImageSearchApi = "cacheContextualWebSearchImageSearchApi";

        ///// <summary>
        ///// Defines list of allowed collections in database
        ///// </summary>
        //private static Dictionary<string, string> _collectionNameMap = new Dictionary<string, string>
        //{
        //    // Identity
        //    { typeof(ApplicationUserEntity).Name,  IdentityUsers },
        //    { typeof(ApplicationRoleEntity).Name,  IdentityRoles },

        //    // Identity Server
        //    { typeof(ApiResource).Name,      IdentityApiResources },
        //    { typeof(Client).Name,           IdentityClients },
        //    { typeof(IdentityResource).Name, IdentityIdentityResources },
        //    { typeof(PersistedGrant).Name,   IdentityPersistedGrant },

        //    // custom entities
        //    { typeof(StudyItemEntity).Name,   StudyItems },

        //    // cache
        //    { typeof(GoogleTranslateDataCacheEntity).Name,   CacheGoogleTranslateApi },
        //    { typeof(ContextualWebSearchImageSearchDataCacheEntity).Name,   CacheContextualWebSearchImageSearchApi },
        //};

        private static List<MongoCollectionConfig> IdentityDbCollectionConfig = new List<MongoCollectionConfig>
        {
            // Identity
            new MongoCollectionConfig
            {
                CollectionType = typeof(ApplicationUserEntity),
                CollectionName = IdentityUsers,
                Indexes = new List<string> {
                }
            },
             new MongoCollectionConfig
            {
                CollectionType = typeof(ApplicationRoleEntity),
                CollectionName = IdentityRoles,
                Indexes = new List<string> {
                }
            },

            // Identity Server
            new MongoCollectionConfig
            {
                CollectionType = typeof(ApiResource),
                CollectionName = IdentityApiResources,
                Indexes = new List<string> {
                }
            },
            new MongoCollectionConfig
            {
                CollectionType = typeof(Client),
                CollectionName = IdentityClients,
                Indexes = new List<string> {
                }
            },
            new MongoCollectionConfig
            {
                CollectionType = typeof(IdentityResource),
                CollectionName = IdentityIdentityResources,
                Indexes = new List<string> {
                }
            },
            new MongoCollectionConfig
            {
                CollectionType = typeof(PersistedGrant),
                CollectionName = IdentityPersistedGrant,
                Indexes = new List<string> {
                }
            },

        };

        private static List<MongoCollectionConfig> MainDbCollectionConfig = new List<MongoCollectionConfig>
        {
            // custom entities
            new MongoCollectionConfig
            {
                CollectionType = typeof(StudyItemEntity),
                CollectionName = StudyItems,
                Indexes = new List<string> {
                }
            },

            // cache
            new MongoCollectionConfig
            {
                CollectionType = typeof(GoogleTranslateDataCacheEntity),
                CollectionName = CacheGoogleTranslateApi,
                Indexes = new List<string> {
                    nameof(GoogleTranslateDataCacheEntity.CacheKey)
                }
            },
            new MongoCollectionConfig
            {
                CollectionType = typeof(ContextualWebSearchImageSearchDataCacheEntity),
                CollectionName = CacheContextualWebSearchImageSearchApi,
                Indexes = new List<string> {
                    nameof(ContextualWebSearchImageSearchDataCacheEntity.CacheKey)
                }
            },
        };

        public static string GetCollectionName<T>(ApplicationDb applicationDb)
        {
            var config = GetCollectionConfig<T>(applicationDb);
            return config.CollectionName;
        }

        public static MongoCollectionConfig GetCollectionConfig<T>(ApplicationDb applicationDb)
        {
            MongoCollectionConfig config = null;

            switch(applicationDb)
            {
                case ApplicationDb.Identity:
                    config = IdentityDbCollectionConfig.FirstOrDefault(x => x.CollectionType == typeof(T));
                    break;
                case ApplicationDb.Main:
                    config = MainDbCollectionConfig.FirstOrDefault(x => x.CollectionType == typeof(T));
                    break;
            }

            if (config == null)
            {
                throw new InvalidOperationException($"Collection config for type {typeof(T).Name} is not registered!");
            }
            return config;
        }

        public static List<MongoCollectionConfig> GetCollectionsConfig(ApplicationDb applicationDb)
        {
            List<MongoCollectionConfig> config = null;

            switch (applicationDb)
            {
                case ApplicationDb.Identity:
                    config = IdentityDbCollectionConfig;
                    break;
                case ApplicationDb.Main:
                    config = MainDbCollectionConfig;
                    break;
            }

            if (config == null)
            {
                throw new InvalidOperationException($"Collections config for {nameof(ApplicationDb)} {applicationDb} is not registered!");
            }
            return config;
        }
    }

    public class MongoCollectionConfig
    {
        public MongoCollectionConfig()
        {
            Indexes = new List<string>();
        }

        public Type CollectionType { get; set; }
        public string CollectionName { get; set; }
        public List<string> Indexes { get; set; } // only 1 filed for now. add compound if needed
    }
}
