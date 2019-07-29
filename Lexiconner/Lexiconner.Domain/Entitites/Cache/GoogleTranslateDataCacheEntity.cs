using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Cache
{
    /// <summary>
    /// Stores chached results from Google Translate API
    /// </summary>
    public class GoogleTranslateDataCacheEntity : DataCacheBaseEntity
    {
        public GoogleTranslateDataCacheEntity(
            List<string> contents,
            string sourceLanguageCode,
            string targetLanguageCode
        )
        {
            Contents = contents;
            SourceLanguageCode = sourceLanguageCode;
            TargetLanguageCode = targetLanguageCode;

            CacheKey = GetCacheKey();
        }

        // api request params
        public List<string> Contents { get; private set; }
        public string SourceLanguageCode { get; private set; }
        public string TargetLanguageCode { get; private set; }

        // api response
        public DataCacheEntity Data { get; set; }

        public override string GetCacheKey()
        {
            return $"{nameof(Contents)}=={string.Join("_", Contents)}__{nameof(SourceLanguageCode)}=={SourceLanguageCode}__{nameof(TargetLanguageCode)}=={TargetLanguageCode}";
        }

        public class DataCacheEntity
        {
            public DataCacheEntity()
            {
                Translations = new List<GoogleTranslateResponseItemEntity>();
                GlossaryTranslations = new List<GoogleTranslateResponseItemEntity>();
            }

            public List<GoogleTranslateResponseItemEntity> Translations { get; set; }
            public List<GoogleTranslateResponseItemEntity> GlossaryTranslations { get; set; }

            public class GoogleTranslateResponseItemEntity
            {
                public string TranslatedText { get; set; }
            }
        }
    }
}
