using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Application.ImportAndExport;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Persistence.Cache;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lexiconner.Application.ApiClients.Dtos.GoogleTranslateResponseDto;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;

namespace Lexiconner.Application.Services
{
    public interface IImageService
    {
        Task<List<ImageSearchResponseItemDto>> FindImagesAsync(string sourceLanguageCode, string imageQuery);
    }

    public class ImageService : IImageService
    {
        private readonly ILogger<IImageService> _logger;
        private readonly IDataCache _dataCache;
        private readonly IGoogleTranslateApiClient _googleTranslateApiClient;
        private readonly IContextualWebSearchApiClient _contextualWebSearchApiClient;

        public ImageService(
            ILogger<IImageService> logger,
            IDataCache dataCache,
            IGoogleTranslateApiClient googleTranslateApiClient,
            IContextualWebSearchApiClient contextualWebSearchApiClient
        )
        {
            _logger = logger;
            _dataCache = dataCache;
            _googleTranslateApiClient = googleTranslateApiClient;
            _contextualWebSearchApiClient = contextualWebSearchApiClient;
        }

        public async Task<List<ImageSearchResponseItemDto>> FindImagesAsync(string sourceLanguageCode, string imageQuery)
        {
            // get translation ru -> en (images can be searched only using en)
            // https://cloud.google.com/translate/docs/languages
            string targetLanguageCode = "en";
            string imageQueryEn = null;
            List<ImageSearchResponseItemDto> result = new List<ImageSearchResponseItemDto>();

            try
            {
                if(String.IsNullOrEmpty(sourceLanguageCode))
                {
                    // detect language
                    string detectLanguageContent = imageQuery;
                    var detectLanguageCache = new GoogleTranslateDetectLangugaeDataCacheEntity(detectLanguageContent); // use just for compare
                    var detectLanguageResultCache = await _dataCache.Get<GoogleTranslateDetectLangugaeDataCacheEntity>(x => x.CacheKey == detectLanguageCache.GetCacheKey());
                    GoogleTranslateDetectLanguageResponseDto detectLanguageResult = null;

                    if (detectLanguageResultCache == null)
                    {
                        // call api
                        _logger.LogInformation($"Calling Google Translate Detect Language API for '{imageQuery}'...");
                        detectLanguageResult = await _googleTranslateApiClient.DetectLanguage(detectLanguageContent);

                        // cache response
                        _logger.LogInformation($"Caching Google Translate Detect Language API response for '{imageQuery}'...");
                        await _dataCache.Add(new GoogleTranslateDetectLangugaeDataCacheEntity(detectLanguageContent)
                        {
                            Data = new GoogleTranslateDetectLangugaeDataCacheEntity.DataCacheEntity
                            {
                                Languages = detectLanguageResult.Languages.Select(x => new GoogleTranslateDetectLangugaeDataCacheEntity.DataCacheEntity.GoogleTranslateDetectLanguageResponseItemEntity
                                {
                                    LanguageCode = x.LanguageCode,
                                    Confidence = x.Confidence,
                                }).ToList()
                            }
                        });
                    }
                    else
                    {
                        // use cache
                        _logger.LogInformation($"Using Google Translate Detect Language API cached response for '{imageQuery}'...");
                        detectLanguageResult = new GoogleTranslateDetectLanguageResponseDto
                        {
                            Languages = detectLanguageResultCache.Data.Languages.Select(x => new GoogleTranslateDetectLanguageResponseDto.GoogleTranslateDetectLanguageResponseItemDto
                            {
                                LanguageCode = x.LanguageCode,
                                Confidence = x.Confidence,
                            }).ToList(),
                        };
                    }

                    if (detectLanguageResult.Languages.Any())
                    {
                        var lang = detectLanguageResult.Languages.Where(x => x.Confidence >= 0.5).FirstOrDefault();
                        if(lang != null)
                        {
                            sourceLanguageCode = lang.LanguageCode;
                        }
                    }
                }

                if(String.IsNullOrEmpty(sourceLanguageCode))
                {
                    // if can't detect - break
                    return result;
                }

                if (sourceLanguageCode != targetLanguageCode)
                {
                    // translate
                    var translateContents = new List<string>() { imageQuery };

                    var translateCache = new GoogleTranslateDataCacheEntity(translateContents, sourceLanguageCode, targetLanguageCode); // use just for compare
                    var translateResultCache = await _dataCache.Get<GoogleTranslateDataCacheEntity>(x => x.CacheKey == translateCache.GetCacheKey());
                    GoogleTranslateResponseDto translateResult = null;

                    if (translateResultCache == null)
                    {
                        // call api
                        _logger.LogInformation($"Calling Google Translate API for '{imageQuery}'...");
                        translateResult = await _googleTranslateApiClient.Translate(translateContents, sourceLanguageCode, targetLanguageCode);

                        // cache response
                        _logger.LogInformation($"Caching Google Translate API response for '{imageQuery}'...");
                        await _dataCache.Add(new GoogleTranslateDataCacheEntity(translateContents, sourceLanguageCode, targetLanguageCode)
                        {
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
                        _logger.LogInformation($"Using Google Translate API cached response for '{imageQuery}'...");
                        translateResult = new GoogleTranslateResponseDto
                        {
                            Translations = translateResultCache.Data.Translations.Select(x => new GoogleTranslateResponseItemDto
                            {
                                TranslatedText = x.TranslatedText
                            }).ToList(),
                            GlossaryTranslations = translateResultCache.Data.GlossaryTranslations.Select(x => new GoogleTranslateResponseItemDto
                            {
                                TranslatedText = x.TranslatedText
                            }).ToList()
                        };
                    }

                    if (translateResult.Translations.Any())
                    {
                        imageQueryEn = translateResult.Translations.First().TranslatedText;
                    }
                }
               


                // make contextual search
                if (!String.IsNullOrEmpty(imageQueryEn))
                {
                    string query = imageQueryEn;
                    int pageNumber = 1;
                    int pageSize = 10;
                    bool isAutoCorrect = false;
                    bool isSafeSearch = false;

                    var imageSearchCache = new ContextualWebSearchImageSearchDataCacheEntity(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch); // use just for compare
                    var imageSearchResultCache = await _dataCache.Get<ContextualWebSearchImageSearchDataCacheEntity>(x => x.CacheKey == imageSearchCache.GetCacheKey());
                    ImageSearchResponseDto imageSearchResult = null;

                    if (imageSearchResultCache == null)
                    {
                        // call api
                        _logger.LogInformation($"Calling Contextual Web Search API for '{imageQuery}' -> '{query}'...");
                        imageSearchResult = await _contextualWebSearchApiClient.ImageSearchAsync(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch);

                        // cache response
                        _logger.LogInformation($"Caching Contextual Web Search API response for '{imageQuery}'-> '{query}'...");
                        await _dataCache.Add(new ContextualWebSearchImageSearchDataCacheEntity(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch)
                        {
                            Data = new ContextualWebSearchImageSearchDataCacheEntity.DataCacheEntity
                            {
                                _Type = imageSearchResult._Type,
                                TotalCount = imageSearchResult.TotalCount,
                                Value = imageSearchResult.Value.Select(x => new ContextualWebSearchImageSearchDataCacheEntity.DataCacheEntity.ImageSearchResponseItemEntity
                                {
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
                        _logger.LogInformation($"Using Contextual Web Search API cached response for '{imageQuery}' -> '{query}'...");
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
                        result.AddRange(imageSearchResult.Value);
                    }
                }
            }
            catch (ApiRateLimitExceededException ex)
            {
                // rethrow
                throw;
            }
            catch (ApiErrorException ex)
            {
                /// rethrow
                throw;
            }
            catch (Exception ex)
            {
                // rethrow
                throw;
            }

            return result;
        }
    }
}
