using Lexiconner.Application.ApiClients;
using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Exceptions;
using Lexiconner.Domain.Entitites.Cache;
using Lexiconner.Persistence.Cache;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static Lexiconner.Application.ApiClients.Dtos.GoogleTranslateResponseDto;
using static Lexiconner.Application.ApiClients.Dtos.ImageSearchResponseDto;

namespace Lexiconner.Application.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Searches images over the internet
        /// </summary>
        Task<IEnumerable<ImageSearchResponseItemDto>> FindImagesAsync(string sourceLanguageCode, string imageQuery, int limit = 10, bool returnOnlyAvailable = true);
        
        /// <summary>
        /// Returns imagesthat can be consumed by the app according to some conditions
        /// </summary>
        IEnumerable<ImageSearchResponseItemDto> GetSuitableImages(IEnumerable<ImageSearchResponseItemDto> imageResult);

        Task<IEnumerable<ImageSearchResponseItemDto>> ExcludeUnavailableImagesAsync(IEnumerable<ImageSearchResponseItemDto> imageResult);
    }

    public class ImageService : IImageService
    {
        private readonly ILogger<IImageService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDataCache _dataCache;
        private readonly IGoogleTranslateApiClient _googleTranslateApiClient;
        private readonly IContextualWebSearchApiClient _contextualWebSearchApiClient;

        public ImageService(
            ILogger<IImageService> logger,
            IHttpClientFactory httpClientFactory,
            IDataCache dataCache,
            IGoogleTranslateApiClient googleTranslateApiClient,
            IContextualWebSearchApiClient contextualWebSearchApiClient
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _dataCache = dataCache;
            _googleTranslateApiClient = googleTranslateApiClient;
            _contextualWebSearchApiClient = contextualWebSearchApiClient;
        }

        public async Task<IEnumerable<ImageSearchResponseItemDto>> FindImagesAsync(string sourceLanguageCode, string imageQuery, int limit = 10, bool returnOnlyAvailable = true)
        {
            imageQuery = imageQuery.ToLowerInvariant();

            // get translation ru -> en (images can be searched only using en)
            // https://cloud.google.com/translate/docs/languages
            string targetLanguageCode = "en";
            string imageQueryEn = null;
            var result = new List<ImageSearchResponseItemDto>();

            try
            {
                if (String.IsNullOrEmpty(sourceLanguageCode))
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

                    //when lang is undefined - don't use it
                    if (!detectLanguageResult.IsUndefinedLanguage && detectLanguageResult.Languages.Any())
                    {
                        var lang = detectLanguageResult.Languages.Where(x => x.Confidence >= 0.5).FirstOrDefault();
                        if (lang != null)
                        {
                            sourceLanguageCode = lang.LanguageCode;
                        }
                    }
                }

                if (String.IsNullOrEmpty(sourceLanguageCode))
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
                else
                {
                    // en already
                    imageQueryEn = imageQuery;
                }

                // make contextual search
                if (!String.IsNullOrEmpty(imageQueryEn))
                {
                    string query = imageQueryEn;
                    int pageNumber = 1; // counting from 1
                    int pageSize = limit;
                    bool isAutoCorrect = false;
                    bool isSafeSearch = true;

                    var imageSearchCache = new ContextualWebSearchImageSearchDataCacheEntity(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch); // use just for compare
                    var imageSearchResultCache = await _dataCache.Get<ContextualWebSearchImageSearchDataCacheEntity>(x => x.CacheKey == imageSearchCache.GetCacheKey());
                    ImageSearchResponseDto imageSearchResult = null;

                    if (imageSearchResultCache == null)
                    {
                        // call api
                        _logger.LogInformation($"Calling Contextual Web Search API for '{imageQuery}' -> '{query}'...");
                        imageSearchResult = await _contextualWebSearchApiClient.ImageSearchAsync(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch);

                        // filter out unavailable images (there are a lot of them usually)
                        if(returnOnlyAvailable)
                        {
                            imageSearchResult.Value = await ExcludeUnavailableImagesAsync(imageSearchResult.Value);
                        }
                        
                        // cache response
                        _logger.LogInformation($"Caching Contextual Web Search API response for '{imageQuery}'-> '{query}'...");
                        await _dataCache.Add(new ContextualWebSearchImageSearchDataCacheEntity(query, pageNumber, pageSize, isAutoCorrect, isSafeSearch)
                        {
                            Data = new ContextualWebSearchImageSearchDataCacheEntity.DataCacheEntity
                            {
                                Type = imageSearchResult.Type,
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
                            Type = imageSearchResultCache.Data.Type,
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
            catch (ApiRateLimitExceededException)
            {
                // rethrow
                throw;
            }
            catch (ApiErrorException)
            {
                /// rethrow
                throw;
            }
            catch (Exception)
            {
                // rethrow
                throw;
            }

            return result;
        }

        public IEnumerable<ImageSearchResponseItemDto> GetSuitableImages(IEnumerable<ImageSearchResponseItemDto> imageResult)
        {
            const int preferredImageWidth = 600;
            const int maxImageWidth = 1400;
            const int preferredImageHeight = 400;
            const int maxImageHeight = 800;

            // try to find suitable image
            if(imageResult == null)
            {
                return new List<ImageSearchResponseItemDto>();
            }
            IEnumerable<ImageSearchResponseItemDto> images = null;
            images = imageResult.Where(x => int.Parse(x.Width) <= preferredImageWidth && int.Parse(x.Height) <= preferredImageHeight);
            images = images.Any() ? images : imageResult.Where(x => int.Parse(x.Width) <= maxImageWidth && int.Parse(x.Height) <= maxImageHeight);

            return images;
        }

        public async Task<IEnumerable<ImageSearchResponseItemDto>> ExcludeUnavailableImagesAsync(IEnumerable<ImageSearchResponseItemDto> imageResults)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var validImages = new ConcurrentBag<ImageSearchResponseItemDto>();
            var actionBlock = new ActionBlock<ImageSearchResponseItemDto>(
                async (image) =>
                {
                    try
                    {
                        var requestMessage = new HttpRequestMessage()
                        {
                            RequestUri = new Uri(image.Url),
                            Method = HttpMethod.Get,
                        };

                        // pretend a normal web browser
                        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

                        var response = await httpClient.SendAsync(requestMessage);
                        string content = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            validImages.Add(image);
                        }

                        // TODO: try to read response or figure out it's an image
                    }
                    catch (HttpRequestException ex)
                    {
                        // ignore
                    }
                },
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = 10,
                }
            );
            await Task.WhenAll(imageResults.Select(x => actionBlock.SendAsync(x)));
            actionBlock.Complete();
            await actionBlock.Completion;

            return validImages.ToList();
        }
    }
}
