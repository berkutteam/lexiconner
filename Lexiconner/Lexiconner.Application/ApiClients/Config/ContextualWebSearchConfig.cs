using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApiClients.Config
{
    public static class ContextualWebSearchConfig
    {
        /// <summary>
        /// These are not coincide with '_type' returned in response
        /// </summary>
        public static class ApiTypesCustom
        {
            public const string AutoComplete = "autocomplete";
            public const string WebSearch = "websearch";
            public const string NewSearch = "newsearch";
            public const string ImageSearch = "imageSearch";
        }

        /// <summary>
        /// These are coincide with '_type' returned in response
        /// </summary>
        public static class ApiTypesNative
        {
            public const string AutoComplete = null; // none
            public const string WebSearch = "all";
            public const string NewSearch = "news";
            public const string ImageSearch = "images";
        }
    }
}
