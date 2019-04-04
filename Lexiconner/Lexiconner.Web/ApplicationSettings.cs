using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Web
{
    public class ApplicationSettings
    {
        public UrlsSettings Urls { get; set; }
    }

    /// <summary>
    /// Settings that can be sent to client
    /// </summary>
    public class ApplicationClientSettings
    {
        public UrlsSettings Urls { get; set; }
    }

    public class UrlsSettings
    {
        public string Api { get; set; }
    }
}
