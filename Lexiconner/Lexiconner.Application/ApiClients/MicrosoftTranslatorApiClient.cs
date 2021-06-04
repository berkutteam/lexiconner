using Lexiconner.Application.ApiClients.Dtos.MicrosoftTranslator;
using Lexiconner.Application.ApplicationSettings;
using Lexiconner.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.ApiClients
{
    // Features: translate text, detect language, transliterate text, dictionary lookup (alternate translations), dictionary examples (translations in context)
    // https://docs.microsoft.com/en-us/azure/cognitive-services/translator/

    public interface IMicrosoftTranslatorApiClient
    {
        Task<MicrosoftTranslatorDictionaryLookupResponseDto> DictionaryLookupAsync(string text, string fromLanguageCode, string toLanguageCode);
    }

    public class MicrosoftTranslatorApiClient : IMicrosoftTranslatorApiClient
    {
        private const string ApiName = "Microsoft Translator Api";

        private readonly MicrosoftTranslatorSettings _settings;
        private readonly ILogger<MicrosoftTranslatorApiClient> _logger;

        private readonly HttpClient _httpClient;

        public MicrosoftTranslatorApiClient(
            IOptions<MicrosoftTranslatorSettings> settings,
            ILogger<MicrosoftTranslatorApiClient> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _settings = settings.Value;
            _logger = logger;

            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-dictionary-lookup
        /// </summary>
        public async Task<MicrosoftTranslatorDictionaryLookupResponseDto> DictionaryLookupAsync(string text, string fromLanguageCode, string toLanguageCode)
        {
            string url = $"{_settings.Endpoint}/dictionary/lookup?api-version={Uri.EscapeDataString(_settings.ApiVersion)}&from={Uri.EscapeDataString(fromLanguageCode)}&to={Uri.EscapeDataString(toLanguageCode)}";
            url = Uri.EscapeUriString(url);

            using var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(
                        // here we can send an array of texts, but we send 1 otem always
                        new []
                        {
                            new
                            {
                                Text = text,
                            }
                        }
                    ), 
                    Encoding.UTF8, "application/json"
                ),
            };
            request.Headers.Add("Ocp-Apim-Subscription-Key", _settings.SubscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", _settings.Region);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var responseErrorDto = JsonConvert.DeserializeObject<MicrosoftTranslatorErrorResponseDto>(responseContent);
                _logger.LogError($"{ApiName} returned error: {JsonConvert.SerializeObject(responseErrorDto.Error)}");
                throw new ApiErrorException($"{ApiName} returned error: {JsonConvert.SerializeObject(responseErrorDto.Error)}");
            }

            var responseDto = JsonConvert.DeserializeObject<IEnumerable<MicrosoftTranslatorDictionaryLookupResponseDto>>(responseContent);
            return responseDto.FirstOrDefault();
        }
    }
}
