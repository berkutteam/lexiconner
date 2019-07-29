using Lexiconner.Application.ApiClients.Dtos;
using Lexiconner.Application.Config;
using Lexiconner.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Lexiconner.Application.Config.RapidApiSettings;

namespace Lexiconner.Application.ApiClients
{
    // TODO - 
    // 1. cache results
    // 2. handle limits

    /// <summary>
    /// https://rapidapi.com/contextualwebsearch/api/web-search
    /// Basic plan is free - 10000 requests/month, then $0.0005 each
    /// </summary>
    public interface IContextualWebSearchApiClient
    {
        Task AutoCompleteAsync();
        Task WebSearchAsync();
        Task NewsSearchAsync();
        Task<ImageSearchResponseDto> ImageSearchAsync(
            string query, // The user's search query string.
            int pageNumber = 1, // The page to view.
            int pageSize = 10, // The number of items per page. The maximum value is 50.
            bool isAutoCorrect = false, // Automatically correct spelling
            bool isSafeSearch = false // Filter results for adult content.    
        );
    }

    public class ContextualWebSearchApiClient : IContextualWebSearchApiClient
    {
        private const int FreeRequestsPerMonth = 10_000;

        private readonly RapidApiSettings _settings;
        private readonly ILogger<IContextualWebSearchApiClient> _logger;

        private readonly HttpClient _httpClient;

        private RapidApiResponseInfoDto _rapidApiResponseInfoDto = null;

        public ContextualWebSearchApiClient(
            RapidApiSettings settings,
            ILogger<IContextualWebSearchApiClient> logger
        )
        {
            _settings = settings;
            _logger = logger;

            _httpClient = new HttpClient(); // TODO use factory
        }

        public Task AutoCompleteAsync()
        {
            CheckApiLimits();
            throw new NotImplementedException();
        }

        public Task WebSearchAsync()
        {
            CheckApiLimits();
            throw new NotImplementedException();
        }

        public Task NewsSearchAsync()
        {
            CheckApiLimits();
            throw new NotImplementedException();
        }

        public async Task<ImageSearchResponseDto> ImageSearchAsync(
            string query, // The user's search query string.
            int pageNumber = 1, // The page to view.
            int pageSize = 10, // The number of items per page. The maximum value is 50. NB: actually returns (pageSize -1)
            bool isAutoCorrect = false, // Automatically correct spelling
            bool isSafeSearch = false // Filter results for adult content.
        )
        {
            CheckApiLimits();

            string autoCorrect = isAutoCorrect ? "true" : "false";
            string safeSearch = isSafeSearch ? "true" : "false";
            string url = $"https://contextualwebsearch-websearch-v1.p.rapidapi.com/api/Search/ImageSearchAPI?autoCorrect={autoCorrect}&pageNumber={pageNumber}&pageSize={pageSize}&q={query}&safeSearch={safeSearch}";
            url = Uri.EscapeUriString(url);

            var request = new HttpRequestMessage(new HttpMethod("GET"), url)
            {
            };
            request.Headers.Add("X-RapidAPI-Key", _settings.ContextualWebSearch.ApplicationKey);

            var response = await _httpClient.SendAsync(request);

            var rapidApiInfo = GetRapidApiResponseInfo(response);
            HandleApiLimits(response, rapidApiInfo);

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<ImageSearchResponseDto>(responseContent);

            return responseDto;
        }

        private RapidApiResponseInfoDto GetRapidApiResponseInfo(HttpResponseMessage httpResponseMessage)
        {
            string XRateLimitRequestsLimit = httpResponseMessage.Headers.GetValues("X-RateLimit-requests-Limit").FirstOrDefault();
            string XRateLimitRequestsRemaining = httpResponseMessage.Headers.GetValues("X-RateLimit-requests-Remaining").FirstOrDefault();

            return new RapidApiResponseInfoDto
            {
                XRapidApiRegion = httpResponseMessage.Headers.GetValues("X-RapidAPI-Region").FirstOrDefault(),
                XRapidApiVersion = httpResponseMessage.Headers.GetValues("X-RapidAPI-Version").FirstOrDefault(),
                XRateLimitRequestsLimit = String.IsNullOrEmpty(XRateLimitRequestsLimit) ? 0 : int.Parse(XRateLimitRequestsLimit),
                XRateLimitRequestsRemaining = String.IsNullOrEmpty(XRateLimitRequestsRemaining) ? 0 : int.Parse(XRateLimitRequestsRemaining),
            };
        }

        private void CheckApiLimits()
        {
            if(_rapidApiResponseInfoDto == null)
            {
                // allow
                return;
            }
            if(_rapidApiResponseInfoDto.XRateLimitRequestsRemaining == 0)
            {
                string message = $"Contextual Web Search: month free quota for BASIC plan is exceeded! Quota = {_rapidApiResponseInfoDto.XRateLimitRequestsLimit}.";
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
                string message = $"Contextual Web Search API returned error response: {responseContent}";
                _logger.LogError(message);
                throw new ApiErrorException(message);
            }
        }
    }
}
