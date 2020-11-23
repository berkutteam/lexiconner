using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites.Cache
{
    /// <summary>
    /// Stores chached results from Contextual Web Search API (image search)
    /// </summary>
    public class ContextualWebSearchImageSearchDataCacheEntity : DataCacheBaseEntity
    {
        public ContextualWebSearchImageSearchDataCacheEntity(
            string query,
            int pageNumber,
            int pageSize,
            bool isAutoCorrect,
            bool isSafeSearch
        )
        {
            Query = query;
            PageNumber = pageNumber;
            PageSize = pageSize;
            IsAutoCorrect = isAutoCorrect;
            IsSafeSearch = isSafeSearch;

            CacheKey = GetCacheKey();
        }

        // api request params
        public string Query { get; private set; }
        public long PageNumber { get; private set; }
        public long PageSize { get; private set; }
        public bool IsAutoCorrect { get; private set; }
        public bool IsSafeSearch { get; private set; }

        // api response
        public DataCacheEntity Data { get; set; }

        public override string GetCacheKey()
        {
            return $"{nameof(Query)}=={Query}__{nameof(PageNumber)}=={PageNumber}__{nameof(PageSize)}=={PageSize}__{nameof(IsAutoCorrect)}=={IsAutoCorrect}__{nameof(IsSafeSearch)}=={IsSafeSearch}";
        }

        public class DataCacheEntity
        {
            public DataCacheEntity()
            {
                Value = new List<ImageSearchResponseItemEntity>();
            }

            // images, news, all (websearch),
            [JsonProperty("Type")]
            [BsonElement("_Type")]
            public string Type { get; set; }
            public long TotalCount { get; set; }
            public List<ImageSearchResponseItemEntity> Value { get; set; }


            public class ImageSearchResponseItemEntity
            {
                public string Url { get; set; }
                public string Height { get; set; }
                public string Width { get; set; }
                public string Thumbnail { get; set; }
                public string ThumbnailHeight { get; set; }
                public string ThumbnailWidth { get; set; }
                public string Base64Encoding { get; set; }
            }
        }
    }
}
