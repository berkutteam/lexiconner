using HtmlAgilityPack;
using Lexiconner.Application.ApiClients.Dtos.ReversoContext;
using Lexiconner.Domain.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Lexiconner.Application.ApiClients.Scrapers
{
    public interface IReversoContextScraper
    {
        Task<ReversoContextTraslationScraperResultDto> GetWordTranslationsAsync(
            string sourceLanguageCode,
            string targetLanguageCode,
            string word
        );
    }

    public class ReversoContextScraper: IReversoContextScraper
    {
        // E.g. https://context.reverso.net/translation/english-russian/cat
        private const string TranslationUrlTemplate = "https://context.reverso.net/translation/{sourceLanguage}-{tragetLanguage}/{word}";

        private readonly ILogger<ReversoContextScraper> _logger;
        private readonly IHttpClientFactory _httpClientFactory = null;

        public ReversoContextScraper(
            ILogger<ReversoContextScraper> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ReversoContextTraslationScraperResultDto> GetWordTranslationsAsync(
            string sourceLanguageCode,
            string targetLanguageCode,
            string word
        )
        {
            // prepare language name
            string sourceLanguage = LanguageConfig.GetLanguageByCode(sourceLanguageCode).IsoLanguageName.ToLowerInvariant();
            string tragetLanguage = LanguageConfig.GetLanguageByCode(targetLanguageCode).IsoLanguageName.ToLowerInvariant();

            using var client = _httpClientFactory.CreateClient();
            string url = TranslationUrlTemplate
                .Replace("{sourceLanguage}", sourceLanguage)
                .Replace("{tragetLanguage}", tragetLanguage)
                .Replace("{word}", word);

            var requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
            };

            // pretend a normal web browser
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0");

            var response = await client.SendAsync(requestMessage);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Request error. Response: {content}");
                throw new Exception($"Error occured in {nameof(ReversoContextScraper.GetWordTranslationsAsync)}.");
            }

            var result = new ReversoContextTraslationScraperResultDto()
            {
                SourceLanguageCode = sourceLanguageCode,
                SourceLanguage = sourceLanguage,
                TargetLanguageCode = targetLanguageCode,
                TargetLanguage = tragetLanguage,
                SearchedWord = word,
            };

            // scrape data from returned HTML
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            const string examplesNodeId = "examples-content";
            const string exampleNodeClass = "example";
            const string exampleSourceNodeClass = "src";
            const string exampleTargeNodeClass = "trg";
            var examplesNode = doc.DocumentNode.SelectSingleNode($"//*[@id=\"{examplesNodeId}\"]");
            var examplesNodes = examplesNode.SelectNodes($"*[contains(concat(' ', normalize-space(@class), ' '), ' {exampleNodeClass} ')]");

            var spacesCleanupRegex = new Regex("\\s{2,}", RegexOptions.IgnoreCase);
            var specialCharsCleanupRegex = new Regex("(\r\n|\r|\n)+", RegexOptions.IgnoreCase);

            foreach (var exampleNode in examplesNodes)
            {
                var sourceSentenceNode = exampleNode.SelectSingleNode($"*[contains(concat(' ', normalize-space(@class), ' '), ' {exampleSourceNodeClass} ')]/*[contains(@class, 'text')]");
                var targetSentenceNode = exampleNode.SelectSingleNode($"*[contains(concat(' ', normalize-space(@class), ' '), ' {exampleTargeNodeClass} ')]/*[contains(@class, 'text')]");

                string sourceLanguageSentence = HttpUtility.HtmlDecode(sourceSentenceNode.InnerText);
                sourceLanguageSentence = spacesCleanupRegex.Replace(sourceLanguageSentence, " ");
                sourceLanguageSentence = specialCharsCleanupRegex.Replace(sourceLanguageSentence, string.Empty);
                sourceLanguageSentence = sourceLanguageSentence.Trim();

                string targetLanguageSentence = HttpUtility.HtmlDecode(targetSentenceNode.InnerText);
                targetLanguageSentence = spacesCleanupRegex.Replace(targetLanguageSentence, " ");
                targetLanguageSentence = specialCharsCleanupRegex.Replace(targetLanguageSentence, string.Empty);
                targetLanguageSentence = targetLanguageSentence.Trim();

                result.Results.Add(new ReversoContextTraslationScraperResultItemDto()
                {
                    SourceLanguageSentence = sourceLanguageSentence,
                    TargetLanguageSentence = targetLanguageSentence,
                });
            }

            return result;
        }
    }
}
