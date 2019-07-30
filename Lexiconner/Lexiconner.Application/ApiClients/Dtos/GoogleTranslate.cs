using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApiClients.Dtos
{
    // https://cloud.google.com/translate/docs/reference/rest/v3beta1/projects.locations/translateText#authorization-scopes

    // https://cloud.google.com/translate/docs/reference/rest/v3beta1/projects/detectLanguage
    #region Detect Languge

    public class GoogleTranslateDetectLanguageRequestDto
    {
        /// <summary>
        /// Required. The content of the input stored as a string.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Optional. The format of the source text, for example, "text/html", "text/plain". If left blank, the MIME type defaults to "text/html".
        /// </summary>
        [JsonProperty("mimeType")]
        public string MimeType { get; set; } = "text/plain";

        /// <summary>
        /// Optional. The language detection model to be used.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }
    }

    public class GoogleTranslateDetectLanguageResponseDto
    {
        public GoogleTranslateDetectLanguageResponseDto()
        {
            Languages = new List<GoogleTranslateDetectLanguageResponseItemDto>();
        }

        /// <summary>
        /// Text translation responses with no glossary applied. This field has the same length as contents.
        /// </summary>
        [JsonProperty("languages")]
        public List<GoogleTranslateDetectLanguageResponseItemDto> Languages { get; set; }

        public class GoogleTranslateDetectLanguageResponseItemDto
        {
            [JsonProperty("languageCode")]
            public string LanguageCode { get; set; }

            [JsonProperty("confidence")]
            public double Confidence { get; set; }
        }
    }


    #endregion


    #region Translate text

    public class GoogleTranslateRequestDto
    {
        /// <summary>
        /// Required. The content of the input in string format. We recommend the total content be less than 30k codepoints. Use locations.batchTranslateText for larger text.
        /// </summary>
        [JsonProperty("contents")]
        public List<string> Contents { get; set; }

        /// <summary>
        /// Optional. The format of the source text, for example, "text/html", "text/plain". If left blank, the MIME type defaults to "text/html".
        /// </summary>
        [JsonProperty("mimeType")]
        public string MimeType { get; set; } = "text/plain";

        /// <summary>
        /// Optional. The BCP-47 language code of the input text if known, for example, "en-US" or "sr-Latn". 
        /// Supported language codes are listed in Language Support. 
        /// If the source language isn't specified, the API attempts to identify the source language automatically and returns the source language within the response.
        /// </summary>
        [JsonProperty("sourceLanguageCode")]
        public string SourceLanguageCode { get; set; }

        /// <summary>
        /// Required. The BCP-47 language code to use for translation of the input text, set to one of the language codes listed in Language Support.
        /// </summary>
        [JsonProperty("targetLanguageCode")]
        public string TargetLanguageCode { get; set; }

        /// <summary>
        /// Optional. The model type requested for this translation.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }
    }

    public class GoogleTranslateResponseDto
    {
        public GoogleTranslateResponseDto()
        {
            Translations = new List<GoogleTranslateResponseItemDto>();
            GlossaryTranslations = new List<GoogleTranslateResponseItemDto>();
        }

        /// <summary>
        /// Text translation responses with no glossary applied. This field has the same length as contents.
        /// </summary>
        [JsonProperty("translations")]
        public List<GoogleTranslateResponseItemDto> Translations { get; set; }

        /// <summary>
        /// Text translation responses if a glossary is provided in the request. This can be the same as translations if no terms apply. This field has the same length as contents.
        /// </summary>
        [JsonProperty("glossaryTranslations")]
        public List<GoogleTranslateResponseItemDto> GlossaryTranslations { get; set; }  

        public class GoogleTranslateResponseItemDto
        {
            [JsonProperty("translatedText")]
            public string TranslatedText { get; set; }
        }
    }

#endregion
}
