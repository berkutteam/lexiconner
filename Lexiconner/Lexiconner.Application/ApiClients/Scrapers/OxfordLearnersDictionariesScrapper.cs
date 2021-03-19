using HtmlAgilityPack;
using Lexiconner.Application.ApiClients.Dtos.OxfordLearnersDictionaries;
using Lexiconner.Application.ApiClients.Dtos.ReversoContext;
using Lexiconner.Domain.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Lexiconner.Application.ApiClients.Scrapers
{
    public interface IOxfordLearnersDictionariesScrapper
    {
        Task<OxfordPronunciationAudioScraperResultDto> GetWordPronunciationAudioAsync(
            string sourceLanguageCode,
            string targetLanguageCode,
            string word
        );
    }

    public class OxfordLearnersDictionariesScrapper : IOxfordLearnersDictionariesScrapper
    {
        // E.g. https://www.oxfordlearnersdictionaries.com/definition/english/distracted?q=Distracted
        private const string TranslationUrlTemplate = "https://www.oxfordlearnersdictionaries.com/definition/{sourceLanguage}/{word}?q={word}";

        private readonly ILogger<OxfordLearnersDictionariesScrapper> _logger;
        private readonly IHttpClientFactory _httpClientFactory = null;

        public OxfordLearnersDictionariesScrapper(
            ILogger<OxfordLearnersDictionariesScrapper> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<OxfordPronunciationAudioScraperResultDto> GetWordPronunciationAudioAsync(
            string sourceLanguageCode,
            string targetLanguageCode,
            string word
        )
        {
            // prepare language name
            string sourceLanguage = LanguageConfig.GetLanguageByCode(sourceLanguageCode).IsoLanguageName.ToLowerInvariant();
            string targetLanguage = LanguageConfig.GetLanguageByCode(targetLanguageCode).IsoLanguageName.ToLowerInvariant();

            var result = new OxfordPronunciationAudioScraperResultDto()
            {
                WordLanguageCode = sourceLanguageCode,
                SourceLanguage = sourceLanguage,
                MeaningLanguageCode = targetLanguageCode,
                TargetLanguage = targetLanguage,
                SearchedWord = word,
                Results = new List<OxfordPronunciationAudioScraperResultItemDto>(),
            };

            // only english allowed as source lang
            if (sourceLanguageCode != "en")
            {
                return result;
            }

            // search only for 1 word
            // NB: for 1+ words the quesry is different
            word = word.ToLowerInvariant();
            var wordParts = word.Split(" ");
            if(wordParts.Length != 1)
            {
                return result;
            }

            using var client = _httpClientFactory.CreateClient();
            string url = TranslationUrlTemplate
                .Replace("{sourceLanguage}", sourceLanguage)
                .Replace("{word}", word);

            var requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };

            // pretend a normal web browser
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

            var response = await client.SendAsync(requestMessage);
            string content = await response.Content.ReadAsStringAsync();

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return result;
            }
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Request error for word {word}. Response: {content}");
                throw new Exception($"Error occured in {nameof(OxfordLearnersDictionariesScrapper.GetWordPronunciationAudioAsync)}.");
            }

            // scrape data from returned HTML
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            const string pronunciationNodeClass = "phonetics";
            const string pronunciationAudioUkNodeClass = "sound audio_play_button pron-uk";
            const string pronunciationAudioUsNodeClass = "sound audio_play_button pron-us";
            const string mp3AudioDataAttribute = "data-src-mp3";
            const string oggAudioDataAttribute = "data-src-ogg";

            var pronunciationNode = doc.DocumentNode.SelectSingleNode($"//*[contains(concat(' ', normalize-space(@class), ' '), ' {pronunciationNodeClass} ')]");

            if (pronunciationNode == null)
            {
                _logger.LogWarning($"Can't find node .{pronunciationNodeClass} ont eh result page.");
                return result;
            }

            // try US first, then UK
            var pronunciationAudioUKNode = pronunciationNode.SelectSingleNode($"//*[contains(concat(' ', normalize-space(@class), ' '), ' {pronunciationAudioUkNodeClass} ')]");
            var pronunciationAudioUSNode = pronunciationNode.SelectSingleNode($"//*[contains(concat(' ', normalize-space(@class), ' '), ' {pronunciationAudioUsNodeClass} ')]");

            string ukAudioMp3Url = null;
            string ukAudioOggUrl = null;
            string usAudioMp3Url = null;
            string usAudioOggUrl = null;

            // when found
            if (pronunciationAudioUKNode != null)
            {
                ukAudioMp3Url = pronunciationAudioUKNode.GetAttributeValue(mp3AudioDataAttribute, null);
                ukAudioOggUrl = pronunciationAudioUKNode.GetAttributeValue(oggAudioDataAttribute, null);
            }
            if (pronunciationAudioUSNode != null)
            {
                usAudioMp3Url = pronunciationAudioUSNode.GetAttributeValue(mp3AudioDataAttribute, null);
                usAudioOggUrl = pronunciationAudioUSNode.GetAttributeValue(oggAudioDataAttribute, null);
            }

            result.Results.Add(new OxfordPronunciationAudioScraperResultItemDto()
            {
                UKAudioMp3Url = ukAudioMp3Url,
                UKAudioOggUrl = ukAudioOggUrl,
                USAudioMp3Url = usAudioMp3Url,
                USAudioOggUrl = usAudioOggUrl,
            });

            return result;
        }
    }
}
