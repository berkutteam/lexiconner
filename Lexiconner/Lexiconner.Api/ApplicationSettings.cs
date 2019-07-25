using Lexiconner.Application.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api
{
    public class ApplicationSettings
    {
        public CorsSettings Cors { get; set; }
        public MongoDbSettings MongoDb { get; set; }
        //public BasicAuthSettings BasicAuth { get; set; }
        public JwtBearerAuthSettings JwtBearerAuth { get; set; }
        public UrlsSettings Urls { get; set; }
        public GoogleSettings Google { get; set; }
    }

    public class CorsSettings
    {
        public CorsSettings()
        {
            AllowedOrigins = new List<string>();
        }

        public List<string> AllowedOrigins { get; set; }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string DatabaseIdentity { get; set; }
    }

    public class BasicAuthSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class JwtBearerAuthSettings
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
    }

    public class UrlsSettings
    {
        public string Self { get; set; }
    }
}
