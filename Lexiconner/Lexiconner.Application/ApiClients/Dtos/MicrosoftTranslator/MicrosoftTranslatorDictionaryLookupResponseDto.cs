using System.Collections.Generic;

namespace Lexiconner.Application.ApiClients.Dtos.MicrosoftTranslator
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-dictionary-lookup#response-body
    /// </summary>
    public class MicrosoftTranslatorDictionaryLookupResponseDto
    {
        public string NormalizedSource { get; set; }
        public string DisplaySource { get; set; }
        public IEnumerable<TranslationItemResponseDto> Translations { get; set; }

        public class TranslationItemResponseDto
        {
            /// <summary>
            /// A string giving the normalized form of this term in the target language. This value should be used as input to lookup examples.
            /// </summary>
            public string NormalizedTarget { get; set; }

            /// <summary>
            /// A string giving the term in the target language and in a form best suited for end-user display. 
            /// Generally, this will only differ from the normalizedTarget in terms of capitalization. 
            /// For example, a proper noun like "Juan" will have normalizedTarget = "juan" and displayTarget = "Juan".
            /// </summary>
            public string DisplayTarget { get; set; }

            /// <summary>
            /// A string associating this term with a part-of-speech tag.
            /// </summary>
            public string PosTag { get; set; }
            public double Confidence { get; set; }
            public string PrefixWord { get; set; }
            public IEnumerable<BackTranslationItemResponseDto> BackTranslations { get; set; }

            public class BackTranslationItemResponseDto
            {
                public string NormalizedText { get; set; }
                public string DisplayText { get; set; }
                public int NumExamples { get; set; }
                public int FrequencyCount { get; set; }
            }
        }
    }
}
