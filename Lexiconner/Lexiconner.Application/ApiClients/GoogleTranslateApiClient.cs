using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Cloud.Translation.V2;
using Lexiconner.Application.ApiClients.Dtos;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using static Lexiconner.Application.Config.GoogleSettings;
using Lexiconner.Application.Exceptions;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Lexiconner.Application.ApiClients
{
    /*
    * 1. Create service account for server ot server calls
    * 2. Get access token
    * 3. Make request
    */

    // TODO - 
    // 1. handle token expired
    // 2. cache results
    // 3. handle limits

    /// <summary>
    /// The free tier is only available for Translation API v3.
    /// </summary>
    public interface IGoogleTranslateApiClient
    {
        Task<GoogleTranslateResponseDto> Translate(List<string> contents, string sourceLanguageCode, string targetLanguageCode);
    }
    
    public class GoogleTranslateApiClient : IGoogleTranslateApiClient
    {
        // https://cloud.google.com/translate/quotas
        private const int FreeCharactersPerMonthLimitV3 = 500_000;

        private const int CharactersPerProjectPerDayDefaultLimitV3 = 1_000_000_000;
        private const int CharactersPerProjectPerDayMaximumLimitV3 = -1; // unlimited

        private const int CharactersPerProjectPer100SecsDefaultLimitV3 = 1_000_000;
        private const int CharactersPerProjectPer100SecsMaximumLimitV3 = 10_000_000;

        //private const int CharactersPerProjectPerUserPer100SecsDefaultLimitV3 = 100_000; // OLD
        //private const int CharactersPerProjectPerUserPer100SecsMaximumLimitV3 = 10_000_000; // OLD

        private const int CharactersPerProjectPerUserPer100SecsDefaultLimitV3 = 1_000_000; // after 2019-07-29
        private const int CharactersPerProjectPerUserPer100SecsMaximumLimitV3 = 10_000_000; // after 2019-07-29

        private readonly string _projectId;
        private readonly GoogleCredentialSettings _googleCredentialSettings;
        private readonly ILogger<IGoogleTranslateApiClient> _logger;

        private readonly HttpClient _httpClient;

        public GoogleTranslateApiClient(
            string projectId,
            GoogleCredentialSettings googleCredentialSettings,
            ILogger<IGoogleTranslateApiClient> logger
        )
        {
            _projectId = projectId;
            _googleCredentialSettings = googleCredentialSettings;
            _logger = logger;

            _httpClient = new HttpClient(); // TODO use factory
        }

        public async Task<GoogleTranslateResponseDto> Translate(List<string> contents, string sourceLanguageCode, string targetLanguageCode)
        {
            string accessToken = await GetAccessToken();

            string url = $"https://translation.googleapis.com/v3beta1/projects/{_projectId}/locations/global:translateText";
            
            var requestDto = new GoogleTranslateRequestDto
            {
                Contents = contents,
                SourceLanguageCode = sourceLanguageCode,
                TargetLanguageCode = targetLanguageCode,
            };
            var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"),
            };
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.SendAsync(request);

            HandleApiLimits(response);

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<GoogleTranslateResponseDto>(responseContent);

            return responseDto;
        }

        /// <summary>
        /// Reads service account (SA) info from json (received after service account creation in console)
        /// and creates signed JWT token with valid scopes specified. SA private key is used to sign token.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {

            string json = await GetServiceAccountJson();
            var credential = GoogleCredential.FromJson(json);
            credential = credential.CreateScoped(new List<string>
                {
                    "https://www.googleapis.com/auth/cloud-translation",
                });

            // looks like for service account authUri can be empty
            // accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(authUri: "https://localhost:5006");
            string accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(authUri: string.Empty);

            return accessToken;
        }

        private async Task<string> GetServiceAccountJson()
        {
            // read from json file
            //string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Secrets/lexiconner-2b83846e6407.json");
            //using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //{
            //    var json = await new StreamReader(stream).ReadToEndAsync();
            //    return json;
            //}

            // read from config
            string json = JsonConvert.SerializeObject(_googleCredentialSettings);
            return json;
        }

        private void HandleApiLimits(HttpResponseMessage httpResponseMessage)
        {
            // 403 - Daily Limit Exceeded - if you exceeded the daily limit
            // 403 - User Rate Limit Exceeded - if you exceeded either of the "Characters per 100 seconds" quotas
            if(httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                var responseContent = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                string message = $"Google Translate API rate limit exceeded: {responseContent}";
                _logger.LogError(message);
                throw new ApiRateLimitExceededException(message);
            }
            else if(!httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                string message = $"Google Translate API returned error response: {responseContent}";
                _logger.LogError(message);
                throw new ApiErrorException(message);
            }
        }
    }
}
