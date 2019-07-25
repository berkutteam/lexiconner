﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApiClients.Dtos
{
    public class ImageSearchResponseDto
    {
        public ImageSearchResponseDto()
        {
            Value = new List<ImageSearchResponseItemDto>();
        }

        // images, news, all (websearch),
        [JsonProperty("_type")]
        public string _Type { get; set; }

        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }

        [JsonProperty("value")]
        public List<ImageSearchResponseItemDto> Value { get; set; }

        // from headers
        public RapidApiResponseInfoDto RapidApiInfo { get; set; }

        public class ImageSearchResponseItemDto
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("height")]
            public string Height { get; set; }

            [JsonProperty("width")]
            public string Width { get; set; }

            [JsonProperty("thumbnail")]
            public string Thumbnail { get; set; }

            [JsonProperty("thumbnailHeight")]
            public string ThumbnailHeight { get; set; }

            [JsonProperty("thumbnailWidth")]
            public string ThumbnailWidth { get; set; }

            [JsonProperty("base64Encoding")]
            public string Base64Encoding { get; set; }
        }
    }
}