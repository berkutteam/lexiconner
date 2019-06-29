﻿using System;
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

    public interface IGoogleTranslateApiClient
    {
        Task<GoogleTranslateResponseDto> Translate(List<string> contents, string sourceLanguageCode, string targetLanguageCode);
    }

    /// <summary>
    /// The free tier is only available for Translation API v3.
    /// </summary>
    public class GoogleTranslateApiClient : IGoogleTranslateApiClient
    {
        private const int FreeCharactersLimitV3 = 500_000;
        private const int CharactersPerProjectPer100SecsLimitV3 = 10_000_000;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _projectId;
        private readonly GoogleCredentialSettings _googleCredentialSettings;

        private readonly HttpClient _httpClient;

        public GoogleTranslateApiClient(
            IHostingEnvironment hostingEnvironment,
            string projectId,
            GoogleCredentialSettings googleCredentialSettings
        )
        {
            _hostingEnvironment = hostingEnvironment;
            _projectId = projectId;
            _googleCredentialSettings = googleCredentialSettings;

            _httpClient = new HttpClient();
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
    }
}
