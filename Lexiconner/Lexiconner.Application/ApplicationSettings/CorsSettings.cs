using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApplicationSettings
{
    public class CorsSettings
    {
        public CorsSettings()
        {
            AllowedOrigins = new List<string>();
        }

        public List<string> AllowedOrigins { get; set; }
    }
}
