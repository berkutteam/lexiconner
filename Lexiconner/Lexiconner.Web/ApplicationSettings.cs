using Lexiconner.Application.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Web
{
    public class ApplicationSettings
    {
        public CorsSettings Cors { get; set; }
        public ClientJwtBearerAuthSettings ClientAuth { get; set; }
        public UrlsSettings Urls { get; set; }
    }

    /// <summary>
    /// Settings that can be sent to client
    /// </summary>
    public class ApplicationClientSettings
    {
        public ClientJwtBearerAuthSettings ClientAuth { get; set; }
        public UrlsSettings Urls { get; set; }
    }

    public class UrlsSettings
    {
        public string ApiExternalUrl { get; set; }
    }

    public class ClientJwtBearerAuthSettings
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string ResponseType { get; set; }
        public List<string> Scopes { get; set; }
        public string PostLogoutRedirectUri { get; set; }
    }
}
