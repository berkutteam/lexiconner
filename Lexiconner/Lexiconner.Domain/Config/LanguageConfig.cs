﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexiconner.Domain.Config
{
    /// <summary>
    /// Based on https://cloud.google.com/translate/docs/languages
    /// Language codes https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
    /// Country codes: https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes
    /// Country icons: https://purecatamphetamine.github.io/country-flag-icons/3x2/index.html
    /// </summary>
    public static class LanguageConfig
    {
        public const string UndefinedLanguageCode = "und";
        public const string EnglishLanguageCode = "en";

        public static IEnumerable<SupportedLanguageModel> SupportedLanguages = new List<SupportedLanguageModel>()
        {
            new SupportedLanguageModel()
            {
                LanguageFamily = "Indo-European",
                IsoLanguageName = "English",
                NativeName = "English",
                Iso639_1_Code = "en",
                CountryIsoAlpha2Code = "GB"
            },
            new SupportedLanguageModel()
            {
                LanguageFamily = "Indo-European",
                IsoLanguageName = "Russian",
                NativeName = "русский",
                Iso639_1_Code = "ru",
                CountryIsoAlpha2Code = "RU"
            },
        };

        public static bool HasLanguageByCode(string code)
        {
            return SupportedLanguages.Any(x => x.Iso639_1_Code == code);
        }

        public static SupportedLanguageModel GetLanguageByCode(string code)
        {
            var language = SupportedLanguages.FirstOrDefault(x => x.Iso639_1_Code == code);
            if(language == null)
            {
                throw new NullReferenceException($"Language with code {code} not found.");
            }
            return language;
        }
    }

    /// <summary>
    /// According to:
    /// https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
    /// </summary>
    public class SupportedLanguageModel
    {
        public string LanguageFamily { get; set; }
        public string IsoLanguageName { get; set; }
        public string NativeName { get; set; }
        public string Iso639_1_Code { get; set; }
        public string CountryIsoAlpha2Code { get; set; }
    }
}
