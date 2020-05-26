using IdentityServer4.Models;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Entitites.IdentityModel;
using Lexiconner.Domain.Models;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories;
using Lexiconner.Persistence.Repositories.MongoDb;
using Lexiconner.Seed.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Lexiconner.Application.ApiClients.Dtos.GoogleTranslateResponseDto;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;

namespace Lexiconner.Seed.Seed
{
    public class SeedServiceDevelopmentLocalhost : ISeedService
    {
        private readonly ApplicationSettings _config;
        private readonly Lexiconner.IdentityServer4.ApplicationSettings _identityConfig;
        private readonly ILogger<ISeedService> _logger;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IDataRepository _dataRepository;
        private readonly IIdentityDataRepository _identityRepository;
        private readonly IIdentityServerConfig _identityServerConfig;
        private readonly IImageService _imageService;

        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly RoleManager<ApplicationRoleEntity> _roleManager;

        public SeedServiceDevelopmentLocalhost(
            IOptions<ApplicationSettings> config,
            IOptions<Lexiconner.IdentityServer4.ApplicationSettings> identityConfig,
            ILogger<ISeedService> logger,
            IWordTxtImporter wordTxtImporter,
            IDataRepository dataRepository,
            IIdentityDataRepository identityRepository,
            IIdentityServerConfig identityServerConfig,
            IImageService imageService,
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<ApplicationRoleEntity> roleManager
        )
        {
            _config = config.Value;
            _identityConfig = identityConfig.Value;
            _logger = logger;
            _wordTxtImporter = wordTxtImporter;
            _dataRepository = dataRepository;
            _identityRepository = identityRepository;
            _identityServerConfig = identityServerConfig;
            _imageService = imageService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task RemoveDatabaseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Start seeding data...");

            await SeedIdentityDb();
            await SeedMainDb();
            
            _logger.LogInformation("Seed completed.");
        }

        private async Task SeedIdentityDb()
        {
            _logger.LogInformation("\n\n");
            _logger.LogInformation("Start seeding identity DB...");

            // Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
            ConfigureMongoDriver2IgnoreExtraElements();

            // Client
            _logger.LogInformation("Clients...");
            foreach (var client in _identityServerConfig.GetClients(_identityConfig))
            {
                if (!_identityRepository.ExistsAsync<ClientEntity>(x => x.Client.ClientId == client.ClientId).GetAwaiter().GetResult())
                {
                    _identityRepository.AddAsync<ClientEntity>(new ClientEntity(client)).GetAwaiter().GetResult();
                }
            }
            _logger.LogInformation("Clients Done.");

            // IdentityResource
            _logger.LogInformation("IdentityResources...");
            foreach (var res in _identityServerConfig.GetIdentityResources())
            {
                if (!_identityRepository.ExistsAsync<IdentityResourceEntity>(x => x.IdentityResource.Name == res.Name).GetAwaiter().GetResult())
                {
                    _identityRepository.AddAsync(new IdentityResourceEntity(res)).GetAwaiter().GetResult();
                }
            }
            _logger.LogInformation("IdentityResources Done.");

            // ApiResource
            _logger.LogInformation("ApiResources...");
            foreach (var api in _identityServerConfig.GetApiResources())
            {
                if (!_identityRepository.ExistsAsync<ApiResourceEntity>(x => x.ApiResource.Name == api.Name).GetAwaiter().GetResult())
                {
                    _identityRepository.AddAsync(new ApiResourceEntity(api)).GetAwaiter().GetResult();
                }
            }
            _logger.LogInformation("ApiResources Done.");


            _logger.LogInformation("Roles...");
            var roles = _identityServerConfig.GetInitialIdentityRoles();

            foreach (var role in roles)
            {
                var existing = _roleManager.FindByNameAsync(role.Name).GetAwaiter().GetResult();
                if (existing == null)
                {
                    _logger.LogInformation($"Role '{role.Name}': creating.");
                    var result = _roleManager.CreateAsync(role);
                    if (!result.Result.Succeeded)
                    {
                        var errorList = result.Result.Errors.ToList();
                        throw new Exception(string.Join("; ", errorList));
                    }
                }
                else
                {
                    _logger.LogInformation($"Role '{role.Name}': exists.");
                }
            }
            _logger.LogInformation("Roles Done.");


            _logger.LogInformation("Users...");
            var users = _identityServerConfig.GetInitialdentityUsers();
            foreach (var user in users)
            {
                var existing = _userManager.FindByEmailAsync(user.Email).GetAwaiter().GetResult();
                if (existing == null)
                {
                    _logger.LogInformation($"User '{user.Email}': creating.");
                    var result = _userManager.CreateAsync(user, _identityServerConfig.DefaultUserPassword);
                    if (!result.Result.Succeeded)
                    {
                        var errorList = result.Result.Errors.ToList();
                        throw new Exception(string.Join("; ", errorList));
                    }
                }
                else
                {
                    _logger.LogInformation($"User '{user.Email}': exists.");
                }
            }
            _logger.LogInformation("Users Done.");

            _logger.LogInformation("Done.");
        }

        private async Task SeedMainDb()
        {
            _logger.LogInformation("\n\n");
            _logger.LogInformation("Start seeding main DB...");

            // seed imported data for marked users
            _logger.LogInformation("StudyItems...");
            var usersWithImport = _identityServerConfig.GetInitialdentityUsers().Where(x => x.IsImportInitialData);

            foreach (var user in usersWithImport)
            {
                // create default custom collections
                var ruWordsCollection = new CustomCollectionEntity()
                {
                    UserId = user.Id,
                    Name = CustomCollectionConfig.RussianWordsCollectionName,
                    IsRoot = false,
                };
                var enWordsCollection = new CustomCollectionEntity()
                {
                    UserId = user.Id,
                    Name = CustomCollectionConfig.EnglishWordsCollectionName,
                    IsRoot = false,
                };
                var rootCollection = new CustomCollectionEntity()
                {
                    UserId = user.Id,
                    Name = CustomCollectionConfig.RootCollectionName,
                    IsRoot = true,
                    Children = new List<CustomCollectionEntity>()
                        {
                            ruWordsCollection,
                            enWordsCollection,
                        },
                };
                var exisintgRootCollection = await _dataRepository.GetOneAsync<CustomCollectionEntity>(x => x.UserId == user.Id && x.IsRoot);
                if (exisintgRootCollection == null)
                {
                    await _dataRepository.AddAsync(rootCollection);
                }
                else
                {
                    rootCollection = exisintgRootCollection;

                    // ru
                    var existingRuWordsCollection = rootCollection.FindCollectionByName(ruWordsCollection.Name);
                    if(existingRuWordsCollection == null)
                    {
                        rootCollection.AddChildCollection(rootCollection.Id, ruWordsCollection);
                        await _dataRepository.UpdateAsync(rootCollection);
                    }
                    else
                    {
                        ruWordsCollection = existingRuWordsCollection;
                    }

                    // en
                    var existingEnWordsCollection = rootCollection.FindCollectionByName(enWordsCollection.Name);
                    if (existingEnWordsCollection == null)
                    {
                        rootCollection.AddChildCollection(rootCollection.Id, enWordsCollection);
                        await _dataRepository.UpdateAsync(rootCollection);
                    }
                    else
                    {
                        enWordsCollection = existingEnWordsCollection;
                    }
                }

                var imports = new[] 
                {
                    new
                    {
                        SourceLanguageCode = "ru",
                        ImportFilePath = _config.Import.RuWordsFilePath,
                        ImportFormat = ".txt",
                        ParentCollectionId = ruWordsCollection.Id,
                    },
                    new
                    {
                        SourceLanguageCode = "en",
                        ImportFilePath = _config.Import.EnWordsFilePath,
                        ImportFormat = ".md",
                        ParentCollectionId = enWordsCollection.Id,
                    }
                };

                // import
                if (!await _dataRepository.ExistsAsync<StudyItemEntity>(x => x.UserId == user.Id))
                {
                    foreach (var import in imports)
                    {
                        await SeedStudyItemsForCollection(
                            user,
                            rootCollection,
                            import.SourceLanguageCode,
                            import.ImportFilePath,
                            import.ImportFormat,
                            import.ParentCollectionId
                        );
                    }
                }
                
            }
            _logger.LogInformation("StudyItems Done.");


            _logger.LogInformation("Done.");
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Client)))
            {
                BsonClassMap.RegisterClassMap<Client>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(ClientEntity)))
            {
                BsonClassMap.RegisterClassMap<ClientEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityResource)))
            {
                BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityResourceEntity)))
            {
                BsonClassMap.RegisterClassMap<IdentityResourceEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(ApiResource)))
            {
                BsonClassMap.RegisterClassMap<ApiResource>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(ApiResourceEntity)))
            {
                BsonClassMap.RegisterClassMap<ApiResourceEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PersistedGrant)))
            {
                BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(PersistedGrantEntity)))
            {
                BsonClassMap.RegisterClassMap<PersistedGrantEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        private async Task SeedStudyItemsForCollection(
            ApplicationUserEntity user,
            CustomCollectionEntity rootCollection,
            string sourceLanguageCode, 
            string importFilePath, 
            string importFormat, 
            string parentCollectionId
        )
        {
            _logger.LogInformation($"Importing StudyItems {sourceLanguageCode}, {importFilePath}, {importFormat}...");
            WordImportResultModel importResult = null;
            if(importFormat == ".txt")
            {
                importResult = await _wordTxtImporter.ImportTxtFormatWords(importFilePath);
            }
            else if (importFormat == ".md")
            {
                importResult = await _wordTxtImporter.ImportMdFormatWords(importFilePath);
            }

            // create imported custom collections
            var collectionMap = new Dictionary<string, CustomCollectionEntity>();
            Action<CustomCollectionEntity, string, IEnumerable<CustomCollectionImportModel>> createCollections = null;
            createCollections = (rootCollectionLocal, parentCollectionIdLocal, collections) =>
            {
                foreach (var collection in collections)
                {
                    var exsitingEntity = rootCollectionLocal.FindCollectionByName(collection.Name);
                    if (exsitingEntity != null)
                    {
                        collectionMap[collection.TempId] = exsitingEntity;
                    }
                    else
                    {
                        var newEntity = new CustomCollectionEntity()
                        {
                            UserId = user.Id,
                            IsRoot = false,
                            Name = collection.Name,
                            Children = new List<CustomCollectionEntity>(), // add in recursive call below
                        };
                        rootCollectionLocal.AddChildCollection(parentCollectionIdLocal, newEntity);
                        collectionMap[collection.TempId] = newEntity;
                    }

                    // add child collections separately
                    var currentEntity = collectionMap[collection.TempId];
                    createCollections(rootCollectionLocal, currentEntity.Id, collection.Children);
                }
            };
            createCollections(rootCollection, parentCollectionId, importResult.Collections);
            await _dataRepository.UpdateAsync(rootCollection);

            var studyItemEntities = importResult.Words.Select(x => 
            {
                List<string> customCollectionIds = new List<string>();
                if(x.CollectionTempId != null && collectionMap.ContainsKey(x.CollectionTempId))
                {
                    customCollectionIds = rootCollection.GetCollectionChainIds(collectionMap[x.CollectionTempId].Id);
                }
                else
                {
                    customCollectionIds = new List<string>() { parentCollectionId };
                }
                return new StudyItemEntity
                {
                    UserId = user.Id,
                    CustomCollectionIds = customCollectionIds,
                    Title = x.Title,
                    Description = x.Description,
                    ExampleTexts = x.ExampleTexts,
                    LanguageCode = sourceLanguageCode,
                    Tags = x.Tags,
                    Image = null,
                };
            }).ToList();

            // fix same ids for different users
            studyItemEntities.ToList().ForEach(x =>
            {
                x.RegenerateId();
                x.Image?.RegenerateId();
            });

            // set images
            await SetStudyItemsImages(studyItemEntities);

            const int chunkSize = 50;
            int chunkCount = (int)(Math.Ceiling((double)studyItemEntities.Count() / (double)chunkSize));
            for (int chunkNumber = 0; chunkNumber < chunkCount; chunkNumber++)
            {
                var items = studyItemEntities.Skip(chunkNumber * chunkSize).Take(chunkSize).ToList();
                _dataRepository.AddManyAsync(items).GetAwaiter().GetResult();
                _logger.LogInformation($"StudyItems processed chunk {chunkNumber + 1}/{chunkCount}.");
            }
            _logger.LogInformation($"StudyItems was added for user #{user.Email}.");
        }

        private async Task SetStudyItemsImages(IEnumerable<StudyItemEntity> studyItemEntities)
        {
            foreach (StudyItemEntity entity in studyItemEntities)
            {
                try
                {
                    var imagesResult = await _imageService.FindImagesAsync(entity.LanguageCode, entity.Title);

                    if (imagesResult.Any())
                    {
                        // try to find suitable image
                        ImageSearchResponseDto.ImageSearchResponseItemDto image = _imageService.GetSuitableImages(imagesResult);

                        if (image != null)
                        {
                            entity.Image = new StudyItemImageEntity
                            {
                                Url = image.Url,
                                Height = image.Height,
                                Width = image.Width,
                                Thumbnail = image.Thumbnail,
                                ThumbnailHeight = image.ThumbnailHeight,
                                ThumbnailWidth = image.ThumbnailWidth,
                                Base64Encoding = image.Base64Encoding,
                            };
                        }
                    }
                }
                catch (ApiRateLimitExceededException ex)
                {
                    _logger.LogWarning($"Api rate limited. {ex.Message}.");
                    continue;
                }
                catch (ApiErrorException ex)
                {
                    // break
                    _logger.LogWarning($"Api error. {ex.Message}.");
                    continue;
                }
                catch (Exception ex)
                {
                    // rethrow
                    _logger.LogError($"Error. {ex.Message}.");
                    throw;
                }
            }

        }
        
    }
}
