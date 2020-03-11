using IdentityServer4.Models;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Entitites.IdentityModel;
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
            IEnumerable<StudyItemEntity> studyItems = null;
            foreach (var user in usersWithImport)
            {
                if (!_dataRepository.ExistsAsync<StudyItemEntity>(x => x.UserId == user.Id).GetAwaiter().GetResult())
                {
                    studyItems = studyItems ?? GetStudyItems().GetAwaiter().GetResult();
                    studyItems = studyItems.Select(x =>
                    {
                        // fix same ids for different users
                        x.RegenerateId();
                        x.Image?.RegenerateId();

                        x.UserId = user.Id;
                        return x;
                    });

                    const int chunkSize = 50;
                    int chunkCount = (int)(Math.Ceiling((double)studyItems.Count() / (double)chunkSize));

                    for (int chunkNumber = 0; chunkNumber < chunkCount; chunkNumber++)
                    {
                        var items = studyItems.Skip(chunkNumber * chunkSize).Take(chunkSize).ToList();
                        _dataRepository.AddManyAsync(items).GetAwaiter().GetResult();
                        _logger.LogInformation($"StudyItems processed chunnk {chunkNumber + 1}/{chunkCount}.");
                    }
                    _logger.LogInformation($"StudyItems was added for user #{user.Email}.");
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

        private async Task<IEnumerable<StudyItemEntity>> GetStudyItems()
        {
            _logger.LogInformation("Importing StudyItems...");
            var wordImports = await _wordTxtImporter.Import();
            var entities = wordImports.Select(x => new StudyItemEntity
            {
                UserId = null,
                Title = x.Word,
                Description = x.Description,
                ExampleText = x.ExampleText,
                LanguageCode = _wordTxtImporter.SourceLanguageCode,
                Tags = x.Tags,
            }).ToList();

            _logger.LogInformation("Making translations and adding images to StudyItems...");

            string sourceLanguageCode = _wordTxtImporter.SourceLanguageCode;
            string targetLanguageCode = "en";

            foreach (StudyItemEntity entity in entities)
            {
                try
                {
                    sourceLanguageCode = "";
                    var imagesResult = await _imageService.FindImagesAsync(sourceLanguageCode, entity.Title);

                    if (imagesResult.Any())
                    {
                        // try to find suitable image
                        ImageSearchResponseDto.ImageSearchResponseItemDto image = _imageService.GetSuitableImages(imagesResult);

                        if(image != null)
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
                    // break
                    return entities;
                }
                catch (ApiErrorException ex)
                {
                    // break
                    return entities;
                }
                catch (Exception ex)
                {
                    // rethrow
                    throw;
                }
            }

            return entities;
        }
    }
}
