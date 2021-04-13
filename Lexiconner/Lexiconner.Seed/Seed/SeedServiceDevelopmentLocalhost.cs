using IdentityServer4.Models;
using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Helpers;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Application.Services;
using Lexiconner.Domain.Config;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Domain.Entitites.General;
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
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using NodaTime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TMDbLib.Client;
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
        private readonly IFilmImporter _filmImporter;
        private readonly IDataRepository _dataRepository;
        private readonly IIdentityDataRepository _identityRepository;
        private readonly IIdentityServerConfig _identityServerConfig;
        private readonly IImageService _imageService;
        private readonly TMDbClient _theMovieDbClient;

        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly RoleManager<ApplicationRoleEntity> _roleManager;

        public SeedServiceDevelopmentLocalhost(
            IOptions<ApplicationSettings> config,
            IOptions<Lexiconner.IdentityServer4.ApplicationSettings> identityConfig,
            ILogger<ISeedService> logger,
            IWordTxtImporter wordTxtImporter,
            IFilmImporter filmImporter,
            IDataRepository dataRepository,
            IIdentityDataRepository identityRepository,
            IIdentityServerConfig identityServerConfig,
            IImageService imageService,
            TMDbClient theMovieDbClient,
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<ApplicationRoleEntity> roleManager
        )
        {
            _config = config.Value;
            _identityConfig = identityConfig.Value;
            _logger = logger;
            _wordTxtImporter = wordTxtImporter;
            _filmImporter = filmImporter;
            _dataRepository = dataRepository;
            _identityRepository = identityRepository;
            _identityServerConfig = identityServerConfig;
            _imageService = imageService;
            _theMovieDbClient = theMovieDbClient;
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
                if (!await _identityRepository.ExistsAsync<ClientEntity>(x => x.Client.ClientId == client.ClientId))
                {
                    await _identityRepository.AddAsync<ClientEntity>(new ClientEntity(client));
                }
            }
            _logger.LogInformation("Clients Done.");

            // IdentityResource
            _logger.LogInformation("IdentityResources...");
            foreach (var res in _identityServerConfig.GetIdentityResources())
            {
                if (!await _identityRepository.ExistsAsync<IdentityResourceEntity>(x => x.IdentityResource.Name == res.Name))
                {
                    await _identityRepository.AddAsync(new IdentityResourceEntity(res));
                }
            }
            _logger.LogInformation("IdentityResources Done.");

            // ApiResource
            _logger.LogInformation("ApiResources...");
            foreach (var api in _identityServerConfig.GetApiResources())
            {
                if (!await _identityRepository.ExistsAsync<ApiResourceEntity>(x => x.ApiResource.Name == api.Name))
                {
                    await _identityRepository.AddAsync(new ApiResourceEntity(api));
                }
            }
            _logger.LogInformation("ApiResources Done.");

            // ApiScope
            _logger.LogInformation("ApiScopes...");
            foreach (var scope in _identityServerConfig.GetApiScopes())
            {
                if (!await _identityRepository.ExistsAsync<ApiScopeEntity>(x => x.ApiScope.Name == scope.Name))
                {
                    await _identityRepository.AddAsync(new ApiScopeEntity(scope));
                }
            }
            _logger.LogInformation("ApiScopes Done.");

            // Roles
            _logger.LogInformation("Roles...");
            var roles = _identityServerConfig.GetInitialIdentityRoles();

            foreach (var role in roles)
            {
                var existing = await _roleManager.FindByNameAsync(role.Name);
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

            // Users
            _logger.LogInformation("Users...");
            var users = _identityServerConfig.GetInitialdentityUsers();
            foreach (var user in users)
            {
                var existing = await _userManager.FindByEmailAsync(user.Email);
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
            var usersWithImport = _identityServerConfig.GetInitialdentityUsers().Where(x => x.IsImportInitialData);

            #region Words
            _logger.LogInformation("Words...");
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
                        WordLanguageCode = "ru",
                        MeaningLanguageCode = "ru",
                        ImportFilePath = _config.Import.RuWordsFilePath,
                        ImportFormat = ".txt",
                        ParentCollectionId = ruWordsCollection.Id,
                    },
                    new
                    {
                        WordLanguageCode = "en",
                        MeaningLanguageCode = default(string),
                        ImportFilePath = _config.Import.EnWordsFilePath,
                        ImportFormat = ".md",
                        ParentCollectionId = enWordsCollection.Id,
                    }
                };

                // import
                var hasSomeWords = await _dataRepository.ExistsAsync<WordEntity>(x => x.UserId == user.Id);
                if (!hasSomeWords || user.IsUpdateExistingDataOnSeed)
                {
                    foreach (var import in imports)
                    {
                        await SeedWordsForCollection(
                            user,
                            rootCollection,
                            import.WordLanguageCode,
                            import.MeaningLanguageCode,
                            import.ImportFilePath,
                            import.ImportFormat,
                            import.ParentCollectionId
                        );
                    }
                }
                
            }
            _logger.LogInformation("Words Done.");

            #endregion

            #region Films

            //_logger.LogInformation("Films...");
            //const string filmsLanguageCode = "ru";
            //const string filmsTz = "Europe/Zaporozhye";

            //// check TMDb configuration
            //var tMDbConfigration = await _theMovieDbClient.GetConfigAsync();
            //int posterWidth = 500;
            //int backdropWidth = 780;
            //int thumbnailWidth = 92; // use poster with small size
            //if (!tMDbConfigration.Images.PosterSizes.Any(x => x == $"w{posterWidth}"))
            //{
            //    throw new Exception($"IMDb doesn't support 'w{posterWidth}' poster size!");
            //}
            //if (!tMDbConfigration.Images.BackdropSizes.Any(x => x == $"w{backdropWidth}"))
            //{
            //    throw new Exception($"IMDb doesn't support 'w{backdropWidth}' backdrop size!");
            //}
            //if (!tMDbConfigration.Images.PosterSizes.Any(x => x == $"w{thumbnailWidth}"))
            //{
            //    throw new Exception($"IMDb doesn't support 'w{thumbnailWidth}' poster size (try to use for thumbnail)!");
            //}

            //// TMDb response cache
            //var tmdbSearchMovieCache = new ConcurrentDictionary<string, TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.Search.SearchMovie>>();
            //var tmdbMovieDetailsCache = new ConcurrentDictionary<int, TMDbLib.Objects.Movies.Movie>();

            //var filmsImportResult = await _filmImporter.ImportTxtFormatFilmsAsync(_config.Import.FilmsFilePath);

            //foreach (var user in usersWithImport)
            //{
            //    if(await _dataRepository.ExistsAsync<UserFilmEntity>(x => x.UserId == user.Id))
            //    {
            //        continue;
            //    }
            //    var filmEntities = filmsImportResult.Select(x =>
            //    {
            //        return new UserFilmEntity
            //        {
            //            UserId = user.Id,
            //            Title = x.Title,
            //            MyRating = x.MyRating,
            //            Comment = x.Comment,
                        
            //            // store in UTC
            //            //WatchedAt = x.WatchedAt == null ? default(DateTimeOffset?) : DateTimeHelper.LocalToUtcOffset(x.WatchedAt.GetValueOrDefault(), filmsTz),
            //            WatchedAt = x.WatchedAt == null ? default(DateTimeOffset?) : x.WatchedAt.Value,
                        
            //            ReleaseYear = x.ReleaseYear,
            //            Genres = x.Genres,
            //            LanguageCode = filmsLanguageCode,
            //        };
            //    }).OrderByDescending(x => x.WatchedAt != null ? x.WatchedAt : DateTimeOffset.MinValue ).ToList();

            //    // fix same ids for different users
            //    filmEntities.ToList().ForEach(x =>
            //    {
            //        x.RegenerateId();
            //        // x.Image?.RegenerateId();
            //    });

            //    // search for movie reference in TMDB
            //    // tMDbConfigration.Images.BaseUrl - has preceeding /
            //    // movieDetails.PosterPath - has leading /
            //    _logger.LogInformation($"Searching films in TMDb...");
            //    var workerBlock = new ActionBlock<UserFilmEntity>(
            //        async (userFilmEntity) =>
            //        {
            //            try
            //            {
            //                _logger.LogInformation($"Searching '{userFilmEntity.Title}'...");
                            
            //                // search movie
            //                var searchMovieResult = tmdbSearchMovieCache.GetValueOrDefault(userFilmEntity.Title);
            //                if(searchMovieResult == null)
            //                {
            //                    searchMovieResult = await _theMovieDbClient.SearchMovieAsync(
            //                       query: userFilmEntity.Title,
            //                       language: userFilmEntity.LanguageCode,
            //                       page: 0,
            //                       includeAdult: false
            //                    );
            //                    tmdbSearchMovieCache.TryAdd(userFilmEntity.Title, searchMovieResult);
            //                }
                            
            //                if (searchMovieResult.Results.Any())
            //                {
            //                    var first = searchMovieResult.Results[0];

            //                    // get movie details
            //                    var movieDetails = tmdbMovieDetailsCache.GetValueOrDefault(first.Id);
            //                    if (movieDetails == null)
            //                    {
            //                        movieDetails = await _theMovieDbClient.GetMovieAsync(first.Id);
            //                        tmdbMovieDetailsCache.TryAdd(first.Id, movieDetails);
            //                    }

            //                    userFilmEntity.Details = new UserFilmDetailsEntity()
            //                    {
            //                        TMDbId = movieDetails.Id,
            //                        IMDbId = movieDetails.ImdbId,
            //                        IsAdult = movieDetails.Adult,
            //                        Budget = movieDetails.Budget,
            //                        Genres = movieDetails.Genres.Select(x => new FilmGenreEntity()
            //                        {
            //                            TMDbGenreId = x.Id,
            //                            Name = x.Name,
            //                        }).ToList(),
            //                        OriginalLanguage = movieDetails.OriginalLanguage,
            //                        OriginalTitle = movieDetails.OriginalTitle,
            //                        ProductionCountries = movieDetails.ProductionCountries.Select(x => new FilmProductionCountryEntity()
            //                        {
            //                            Iso_3166_1 = x.Iso_3166_1,
            //                            Name = x.Name,
            //                        }).ToList(),
            //                        ReleaseDate = movieDetails.ReleaseDate,
            //                        Revenue = movieDetails.Revenue,
            //                        Status = movieDetails.Status,
            //                        Title = movieDetails.Title,
            //                        VoteAverage = movieDetails.VoteAverage,
            //                        VoteCount = movieDetails.VoteCount,
            //                        Image = new UserFilmImageEntity()
            //                        {
            //                            PosterUrl = $"{tMDbConfigration.Images.BaseUrl}w{posterWidth}{movieDetails.PosterPath}",
            //                            PosterWidth = posterWidth,
            //                            BackdropUrl = $"{tMDbConfigration.Images.BaseUrl}w{backdropWidth}{movieDetails.BackdropPath}",
            //                            BackdropWidth = backdropWidth,
            //                            ThumbnailUrl = $"{tMDbConfigration.Images.BaseUrl}w{thumbnailWidth}{movieDetails.PosterPath}",
            //                            ThumbnailWidth = thumbnailWidth,
            //                        },
            //                    };
            //                }
            //            }
            //            catch(TMDbLib.Objects.Exceptions.GeneralHttpException ex)
            //            {
            //                _logger.LogError(ex, $"TMDb error for '{userFilmEntity.Title}'. Error: {ex.Message}");
            //                // skip
            //            }
                        
            //        },
            //        new ExecutionDataflowBlockOptions()
            //        {
            //            MaxDegreeOfParallelism = 10,
            //        }
            //    );
            //    await Task.WhenAll(filmEntities.Select(x => workerBlock.SendAsync(x)));
            //    workerBlock.Complete();
            //    await workerBlock.Completion;

            //    const int chunkSize = 50;
            //    int chunkCount = (int)(Math.Ceiling((double)filmEntities.Count() / (double)chunkSize));
            //    for (int chunkNumber = 0; chunkNumber < chunkCount; chunkNumber++)
            //    {
            //        var items = filmEntities.Skip(chunkNumber * chunkSize).Take(chunkSize).ToList();
            //        await _dataRepository.AddManyAsync(items);
            //        _logger.LogInformation($"Films processed chunk {chunkNumber + 1} / {chunkCount}.");
            //    }
            //    _logger.LogInformation($"Films were added for user #{user.Email}.");
            //}
            //_logger.LogInformation("Films Done.");

            #endregion

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

            if (!BsonClassMap.IsClassMapRegistered(typeof(ApiScope)))
            {
                BsonClassMap.RegisterClassMap<ApiScope>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(ApiScopeEntity)))
            {
                BsonClassMap.RegisterClassMap<ApiScopeEntity>(cm =>
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

        private async Task SeedWordsForCollection(
            ApplicationUserEntity user,
            CustomCollectionEntity rootCollection,
            string sourceLanguageCode, 
            string meaningLanguageCode,
            string importFilePath, 
            string importFormat, 
            string parentCollectionId
        )
        {
            _logger.LogInformation($"Importing Words {sourceLanguageCode}, {meaningLanguageCode}, {importFilePath}, {importFormat}...");

            // import from file
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

            // build words
            var wordEntities = importResult.Words.Select(x => 
            {
                List<string> customCollectionIds;
                if(x.CollectionTempId != null && collectionMap.ContainsKey(x.CollectionTempId))
                {
                    customCollectionIds = rootCollection.GetCollectionChainIds(collectionMap[x.CollectionTempId].Id);
                }
                else
                {
                    customCollectionIds = new List<string>() { parentCollectionId };
                }

                return new WordEntity
                {
                    UserId = user.Id,
                    CustomCollectionIds = customCollectionIds,
                    Word = x.Title,
                    Meaning = x.Description,
                    Examples = x.ExampleTexts,
                    WordLanguageCode = sourceLanguageCode,
                    MeaningLanguageCode = meaningLanguageCode, // TODO for EN words where RU and EN meanings
                    Tags = x.Tags,
                    Images = x.ImageUrls.Select(imageUrl => new GeneralImageEntity()
                    {
                        IsAddedByUrl = true,
                        Url = imageUrl,
                        // set other fields later
                    }).ToList(),
                };
            }).ToList();

            // set images
            await SetWordsImages(wordEntities);

            // fix same ids for different users
            wordEntities.ToList().ForEach(x =>
            {
                x.RegenerateId();
                x.Images.ForEach(x => x.RegenerateId());
            });

            // save words
            const int chunkSize = 50;
            int chunkCount = (int)(Math.Ceiling((double)wordEntities.Count / (double)chunkSize));
            IEnumerable<WordEntity> userExistingWords = null;
            for (int chunkNumber = 0; chunkNumber < chunkCount; chunkNumber++)
            {
                var items = wordEntities.Skip(chunkNumber * chunkSize).Take(chunkSize).ToList();

                // only unexisting
                if (userExistingWords == null)
                {
                    userExistingWords = await _dataRepository.GetManyAsync<WordEntity>(x => x.UserId == user.Id);
                }
                var itemsToCreate = new List<WordEntity>();
                foreach (var item in items)
                {
                    var exists = userExistingWords.Any(x => x.Word == item.Word);
                    if (!exists)
                    {
                        itemsToCreate.Add(item);
                    }
                    else if(exists && user.IsUpdateExistingDataOnSeed)
                    {
                        await _dataRepository.DeleteAsync<WordEntity>(x => x.Word == item.Word);
                        itemsToCreate.Add(item);
                    }
                }

                await _dataRepository.AddManyAsync(itemsToCreate);

                _logger.LogInformation($"Words processed chunk {chunkNumber + 1} / {chunkCount}.");
            }
            _logger.LogInformation($"Words were added for user #{user.Email}.");
        }

        private static Dictionary<Tuple<string, string>, IEnumerable<ImageSearchResponseItemDto>> ImagesCache = new Dictionary<Tuple<string, string>, IEnumerable<ImageSearchResponseItemDto>>();
        private async Task SetWordsImages(IEnumerable<WordEntity> wordEntities)
        {
            foreach (WordEntity entity in wordEntities)
            {
                try
                {
                    // handle with images added by URLs
                    if(entity.Images.Any(x => x.IsAddedByUrl))
                    {
                        entity.Images = (await Task.WhenAll(entity.Images.Select(async image =>
                        {
                            if (!image.IsAddedByUrl)
                            {
                                return image;
                            }
                            var imageInfo = await _imageService.GetImageInfoByUrlAsync(image.Url);
                            if (imageInfo == null)
                            {
                                return null;
                            }

                            image.Width = imageInfo.Width;
                            image.Height = imageInfo.Height;
                            return image;
                        }))).Where(x => x != null).ToList();
                        continue;
                    }

                    // search for images
                    IEnumerable<ImageSearchResponseItemDto> imagesResults = null;
                    var cacheKey = new Tuple<string, string>(entity.WordLanguageCode, entity.Word);
                    if (ImagesCache.ContainsKey(cacheKey))
                    {
                        imagesResults = ImagesCache[cacheKey];
                    }
                    else
                    {
                        imagesResults = await _imageService.FindImagesAsync(entity.WordLanguageCode, entity.Word);
                        imagesResults = _imageService.GetSuitableImages(imagesResults);
                        ImagesCache.Add(cacheKey, imagesResults);
                    }

                    if (imagesResults.Any())
                    {
                        // try to find suitable image
                        var image = imagesResults.FirstOrDefault();

                        if (image != null)
                        {
                            entity.Images.Add(new GeneralImageEntity
                            {
                                Url = image.Url,
                                Height = int.Parse(image.Height),
                                Width = int.Parse(image.Width),
                                Thumbnail = image.Thumbnail,
                                ThumbnailHeight = int.Parse(image.ThumbnailHeight),
                                ThumbnailWidth = int.Parse(image.ThumbnailWidth),
                                Base64Encoding = image.Base64Encoding,
                            });
                        }
                    }
                }
                catch (ApiRateLimitExceededException ex)
                {
                    _logger.LogWarning($"Api rate limited. {ex.Message}.");
                }
                catch (ApiErrorException ex)
                {
                    // break
                    _logger.LogWarning($"Api error. {ex.Message}.");
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
