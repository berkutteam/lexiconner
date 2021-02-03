using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApplicationSettings
{
    public class RapidApiSettings
    {
        public ContextualWebSearchApiSettings ContextualWebSearch { get; set; }
        public TwinwordWordDictionaryApiSettings TwinwordWordDictionary { get; set; }
    }
}
