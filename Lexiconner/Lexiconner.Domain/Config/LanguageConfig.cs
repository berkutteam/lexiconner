using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexiconner.Domain.Config
{
    /// <summary>
    /// Based on https://cloud.google.com/translate/docs/languages
    /// </summary>
    public static class LanguageConfig
    {
        public const string UndefinedLanguageCode = "und";

        public static IEnumerable<SupportedLanguageModel> SupportedLanguages = new List<SupportedLanguageModel>()
        {
            new SupportedLanguageModel()
            {
                LanguageFamily = "Indo-European",
                IsoLanguageName = "English",
                NativeName = "English",
                Iso639_1_Code = "en",
            },
            new SupportedLanguageModel()
            {
                LanguageFamily = "Indo-European",
                IsoLanguageName = "Russian",
                NativeName = "русский",
                Iso639_1_Code = "ru",
            },
        };

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
    }
}
