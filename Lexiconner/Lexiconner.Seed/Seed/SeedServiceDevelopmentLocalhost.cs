using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.IdentityServer4.Config;
using Lexiconner.Persistence.Cache;
using Lexiconner.Persistence.Repositories.Base;
using Lexiconner.Seed.Models;
using Lexiconner.Seed.Seed.ImportAndExport;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ISeedService> _logger;
        private readonly IWordTxtImporter _wordTxtImporter;
        private readonly IMongoRepository _mongoRepository;
        private readonly IIdentityRepository _identityRepository;
        private readonly IDataCache _dataCache;
        private readonly IIdentityServerConfig _identityServerConfig;
        private readonly IGoogleTranslateApiClient _googleTranslateApiClient;
        private readonly IContextualWebSearchApiClient _contextualWebSearchApiClient;

        public SeedServiceDevelopmentLocalhost(
            ILogger<ISeedService> logger,
            IWordTxtImporter wordTxtImporter,
            IMongoRepository mongoRepository,
            IIdentityRepository identityRepository,
            IDataCache dataCache,
            IIdentityServerConfig identityServerConfig,
            IGoogleTranslateApiClient googleTranslateApiClient,
            IContextualWebSearchApiClient contextualWebSearchApiClient
        )
        {
            _logger = logger;
            _wordTxtImporter = wordTxtImporter;
            _mongoRepository = mongoRepository;
            _identityRepository = identityRepository;
            _dataCache = dataCache;
            _identityServerConfig = identityServerConfig;
            _googleTranslateApiClient = googleTranslateApiClient;
            _contextualWebSearchApiClient = contextualWebSearchApiClient;
        }

        public Task RemoveDatabaseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Start seeding data...");

            _logger.LogInformation("Users...");
          
            _logger.LogInformation("Users Done.");

            // seed imported data for marked users
            _logger.LogInformation("StudyItems...");
            var usersWithImport = _identityServerConfig.GetInitialdentityUsers().Where(x => x.IsImportInitialData);
            IEnumerable<StudyItemEntity> studyItems = null;
            foreach (var user in usersWithImport)
            {
                if (!_mongoRepository.AnyAsync<StudyItemEntity>(x => x.UserId == user.Id).GetAwaiter().GetResult())
                {
                    studyItems = studyItems ?? GetStudyItems().GetAwaiter().GetResult();
                    studyItems = studyItems.Select(x =>
                    {
                        x.UserId = user.Id;
                        return x;
                    });

                    const int chunkSize = 50;
                    int chunkCount = (int)(Math.Ceiling((double)studyItems.Count() / (double)chunkSize));

                    for (int chunkNumber = 0; chunkNumber < chunkCount; chunkNumber++)
                    {
                        var items = studyItems.Skip(chunkNumber * chunkSize).Take(chunkSize);
                        _mongoRepository.AddAsync(items).GetAwaiter().GetResult();
                        _logger.LogInformation($"StudyItems processed chunnk {chunkNumber + 1}/{chunkCount}.");
                    }
                    _logger.LogInformation($"StudyItems was added for user #{user.Email}.");
                }
            }
            _logger.LogInformation("StudyItems Done.");

            _logger.LogInformation("Seed completed.");
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
                Tags = x.Tags,
            });

            _logger.LogInformation("Making translations and adding images to StudyItems...");

            // get translation ru -> en
            // https://cloud.google.com/translate/docs/languages
            string sourceLanguageCode = _wordTxtImporter.SourceLanguageCode;
            string targetLanguageCode = "en";
            
            foreach (var entity in entities)
            {
                try
                {
                    // translate
                    var translateContents = new List<string>() { entity.Title };

                    var translateCache = new GoogleTranslateDataCacheEntity(translateContents, sourceLanguageCode, targetLanguageCode); // use just for compare
                    var translateResultCache = await _dataCache.Get<GoogleTranslateDataCacheEntity>(x => x.CacheKey == translateCache.GetCacheKey());
                    GoogleTranslateResponseDto translateResult = null;

                    if (translateResultCache == null)
                    {
                        // call api
                        _logger.LogInformation($"Calling Google Translate API for '{entity.Title}'...");
                        translateResult = await _googleTranslateApiClient.Translate(translateContents, sourceLanguageCode, targetLanguageCode);

                        // cache response
                        _logger.LogInformation($"Caching Google Translate API response for '{entity.Title}'...");
                        await _dataCache.Add(new GoogleTranslateDataCacheEntity(translateContents, sourceLanguageCode, targetLanguageCode) {
                            Data = new GoogleTranslateDataCacheEntity.DataCacheEntity
                            {
                                Translations = translateResult.Translations.Select(x => new GoogleTranslateDataCacheEntity.DataCacheEntity.GoogleTranslateResponseItemEntity
                                {
                                    TranslatedText = x.TranslatedText
                                }).ToList(),
                                GlossaryTranslations = translateResult.GlossaryTranslations.Select(x => new GoogleTranslateDataCacheEntity.DataCacheEntity.GoogleTranslateResponseItemEntity
                                {
                                    TranslatedText = x.TranslatedText
                                }).ToList(),
                            }
                        });
                    }
                    else
                    {
                        // use cache
                        _logger.LogInformation($"Using Google Translate API cached response for '{entity.Title}'...");
                        translateResult = new GoogleTranslateResponseDto
                        {
                            Translations = translateResultCache.Data.Translations.Select(x => new GoogleTranslateResponseItemDto {
                                TranslatedText = x.TranslatedText
                            }).ToList(),
                            GlossaryTranslations = translateResultCache.Data.GlossaryTranslations.Select(x => new GoogleTranslateResponseItemDto
                            {
                                TranslatedText = x.TranslatedText
                            }).ToList()
                        };
                    }


                    // make contextual search
                    if (translateResult.Translations.Any())
                    {
                        string query = translateResult.Translations.First().TranslatedText;
                        int pageNumber = 1;
                        int pageSize = 2;
                        bool isAutoCorrect = false;
                        bool isSafeSearch = false;

                        var imageSearchCache = new ContextualWebSearchImageSearchDataCacheEntity(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch); // use just for compare
                        var imageSearchResultCache = await _dataCache.Get<ContextualWebSearchImageSearchDataCacheEntity>(x => x.CacheKey == imageSearchCache.GetCacheKey());
                        ImageSearchResponseDto imageSearchResult = null;

                        if (imageSearchResultCache == null)
                        {
                            // call api
                            _logger.LogInformation($"Calling Contextual Web Search API for '{entity.Title}' -> '{query}'...");
                            imageSearchResult = await _contextualWebSearchApiClient.ImageSearchAsync(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch);

                            // cache response
                            _logger.LogInformation($"Caching Contextual Web Search API response for '{entity.Title}'-> '{query}'...");
                            await _dataCache.Add(new ContextualWebSearchImageSearchDataCacheEntity(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch)
                            {
                                Data = new ContextualWebSearchImageSearchDataCacheEntity.DataCacheEntity
                                {
                                    _Type = imageSearchResult._Type,
                                    TotalCount = imageSearchResult.TotalCount,
                                    Value = imageSearchResult.Value.Select(x => new ContextualWebSearchImageSearchDataCacheEntity.DataCacheEntity.ImageSearchResponseItemEntity {
                                        Url = x.Url,
                                        Height = x.Height,
                                        Width = x.Width,
                                        Thumbnail = x.Thumbnail,
                                        ThumbnailHeight = x.ThumbnailHeight,
                                        ThumbnailWidth = x.ThumbnailWidth,
                                        Base64Encoding = x.Base64Encoding,
                                    }).ToList()
                                }
                            });
                        }
                        else
                        {
                            // use cache
                            _logger.LogInformation($"Using Contextual Web Search API cached response for '{entity.Title}' -> '{query}'...");
                            imageSearchResult = new ImageSearchResponseDto
                            {
                                _Type = imageSearchResultCache.Data._Type,
                                TotalCount = imageSearchResultCache.Data.TotalCount,
                                Value = imageSearchResultCache.Data.Value.Select(x => new ImageSearchResponseItemDto
                                {
                                    Url = x.Url,
                                    Height = x.Height,
                                    Width = x.Width,
                                    Thumbnail = x.Thumbnail,
                                    ThumbnailHeight = x.ThumbnailHeight,
                                    ThumbnailWidth = x.ThumbnailWidth,
                                    Base64Encoding = x.Base64Encoding,
                                }).ToList()
                            };
                        }

                        if (imageSearchResult.Value.Any())
                        {
                            var image = imageSearchResult.Value.First();
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
