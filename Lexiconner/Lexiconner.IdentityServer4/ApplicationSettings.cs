using Lexiconner.Application.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4
{
    public class ApplicationSettings
    {
        public CorsSettings Cors { get; set; }
        public MongoDbSettings MongoDb { get; set; }
        public UrlSettings Urls { get; set; }
        public IdenitytServer4Settings IdenitytServer4 { get; set; }
    }

    public class CorsSettings
    {
        public CorsSettings()
        {
            AllowedOrigins = new List<string>();
        }

        public List<string> AllowedOrigins { get; set; }
    }

    public class UrlSettings
    {
        public string WebApi { get; set; }
        public string WebSpa { get; set; }
        public string WebTestSpa { get; set; }
    }

    public class IdenitytServer4Settings
    {
        public SigningCredentialSettings SigningCredential { get; set; }

        public class SigningCredentialSettings
        {
            /// <summary>
            /// issuer name in X509Store to find certificate
            /// </summary>
            public string KeyStoreIssuer { get; set; }

            /// <summary>
            /// path to .pfx X509 certificate
            /// </summary>
            public string KeyFilePath { get; set; }

            /// <summary>
            /// .pfx X509 certificate password
            /// </summary>
            public string KeyFilePassword { get; set; }

            /// <summary>
            /// path to developer .pfx X509 certificate
            /// </summary>
            public string KeyFilePathDeveloper { get; set; }
        }
    }
}
