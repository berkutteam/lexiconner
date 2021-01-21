using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApplicationSettings
{
    public class RapidApiSettings
    {
        public SpecificRapidApiSettings ContextualWebSearch { get; set; }

        public class SpecificRapidApiSettings
        {
            public string ApplicationKey { get; set; }
        }
    }
}
