using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.ApiClients.Dtos.TwinwordWordDictionary;
using Lexiconner.Application.ApplicationSettings;
using Lexiconner.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lexiconner.Application.ApiClients
{
    /// <summary>
    /// https://rapidapi.com/twinword/api/word-dictionary?endpoint=53aa5089e4b07e1f4ebeb443
    /// </summary>
    public interface ITwinwordWordDictionaryApiClient
    {
        /// <summary>
        /// Get the broad terms, narrow terms, related terms, evocations, synonyms, associations, and derived terms of a word.
        /// </summary>
        Task<ReferenceResponseDto> GetReferenceAsync(string entry);
    }

    public class TwinwordWordDictionaryApiClient : ITwinwordWordDictionaryApiClient
    {
        private const int FreeRequestsPerMonth = 10_000;
        private const string ApiName = "Twinword Word Dictionary API";

        private readonly TwinwordWordDictionaryApiSettings _settings;
        private readonly ILogger<ITwinwordWordDictionaryApiClient> _logger;

        private readonly HttpClient _httpClient;

        private RapidApiResponseInfoDto _rapidApiResponseInfoDto = null;

        public TwinwordWordDictionaryApiClient(
            TwinwordWordDictionaryApiSettings settings,
            ILogger<ITwinwordWordDictionaryApiClient> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _settings = settings;
            _logger = logger;

            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<ReferenceResponseDto> GetReferenceAsync(string entry)
        {
            CheckApiLimits();

            string url = $"{_settings.ApiUrl}/reference/?entry={entry}";
            url = Uri.EscapeUriString(url);

            var request = new HttpRequestMessage(new HttpMethod("GET"), url)
            {
            };
            request.Headers.Add("X-RapidAPI-Key", _settings.RapidApiKey);
            request.Headers.Add("X-RapidAPI-Host", _settings.RapidApiHost);

            var response = await _httpClient.SendAsync(request);

            var rapidApiInfo = GetRapidApiResponseInfo(response);
            HandleApiLimits(response, rapidApiInfo);

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<ReferenceResponseDto>(responseContent);

            return responseDto;
        }

        private static RapidApiResponseInfoDto GetRapidApiResponseInfo(HttpResponseMessage httpResponseMessage)
        {
            string XRateLimitRequestsLimit = 
                httpResponseMessage.Headers.GetValues("X-RateLimit-requests-Limit").FirstOrDefault() ?? 
                httpResponseMessage.Headers.GetValues("x-ratelimit-requests-limit").FirstOrDefault();
            string XRateLimitRequestsRemaining = 
                httpResponseMessage.Headers.GetValues("X-RateLimit-requests-Remaining").FirstOrDefault() ??
                httpResponseMessage.Headers.GetValues("x-ratelimit-requests-remaining").FirstOrDefault();

            return new RapidApiResponseInfoDto
            {
                XRapidApiRegion = httpResponseMessage.Headers.GetValues("X-RapidAPI-Region").FirstOrDefault() ?? httpResponseMessage.Headers.GetValues("x-rapidapi-region").FirstOrDefault(),
                XRapidApiVersion = httpResponseMessage.Headers.GetValues("X-RapidAPI-Version").FirstOrDefault() ?? httpResponseMessage.Headers.GetValues("x-rapidapi-version").FirstOrDefault(),
                XRateLimitRequestsLimit = String.IsNullOrEmpty(XRateLimitRequestsLimit) ? 0 : int.Parse(XRateLimitRequestsLimit),
                XRateLimitRequestsRemaining = String.IsNullOrEmpty(XRateLimitRequestsRemaining) ? 0 : int.Parse(XRateLimitRequestsRemaining),
            };
        }

        private void CheckApiLimits()
        {
            if (_rapidApiResponseInfoDto == null)
            {
                // allow
                return;
            }
            if (_rapidApiResponseInfoDto.XRateLimitRequestsRemaining == 0)
            {
                string message = $"{ApiName}: month free quota for BASIC plan is exceeded! Quota = {_rapidApiResponseInfoDto.XRateLimitRequestsLimit}.";
                _logger.LogError(message);
                throw new ApiRateLimitExceededException(message);
            }
        }

        private void HandleApiLimits(HttpResponseMessage httpResponseMessage, RapidApiResponseInfoDto rapidApiResponseInfoDto)
        {
            _rapidApiResponseInfoDto = rapidApiResponseInfoDto;

            if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                // TODO check if api suspended. Need to figure out how to check that (what Status?)
                //var responseContent = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                //string message = $"Contextual Web Search API rate limit exceeded: {responseContent}";
                //_logger.LogError(message);
                //throw new ApiRateLimitExceededException(message);
            }
            else if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                string message = $"{ApiName} returned error response: {responseContent}";
                _logger.LogError(message);
                throw new ApiErrorException(message);
            }
        }
    }
}
