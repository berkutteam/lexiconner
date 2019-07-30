using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Cache
{
    /// <summary>
    /// Stores chached results from Google Translate DetectLanguage API
    /// </summary>
    public class GoogleTranslateDetectLangugaeDataCacheEntity : DataCacheBaseEntity
    {
        public GoogleTranslateDetectLangugaeDataCacheEntity(
            string content
        )
        {
            Content = content;

            CacheKey = GetCacheKey();
        }

        // api request params
        public string Content { get; private set; }

        // api response
        public DataCacheEntity Data { get; set; }

        public override string GetCacheKey()
        {
            return $"{nameof(Content)}=={Content}";
        }

        public class DataCacheEntity
        {
            public DataCacheEntity()
            {
                Languages = new List<GoogleTranslateDetectLanguageResponseItemEntity>();
            }

            public List<GoogleTranslateDetectLanguageResponseItemEntity> Languages { get; set; }

            public class GoogleTranslateDetectLanguageResponseItemEntity
            {
                public string LanguageCode { get; set; }
                public double Confidence { get; set; }
            }
        }
    }
}
