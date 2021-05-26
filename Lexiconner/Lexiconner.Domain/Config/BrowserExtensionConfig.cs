using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Config
{
    public static class BrowserExtensionConfig
    {
        public static IEnumerable<BrowserExtensionConfigVersion> Versions = new List<BrowserExtensionConfigVersion>()
        {
            new BrowserExtensionConfigVersion()
            {
                Version = "0.1.0",
                IsSupported = true,
            },
        };
    }

    public class BrowserExtensionConfigVersion 
    {
        public string Version { get; set; }
        public bool IsSupported { get; set; }
    }
}
