using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.IdentityServer4
{
    public class ApplicationSettings
    {
        public MongoDbSettings MongoDb { get; set; }
        public UrlSettings Urls { get; set; }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }

    public class UrlSettings
    {
        public string WebApi { get; set; }
        public string WebSpa { get; set; }
    }
}
