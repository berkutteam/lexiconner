using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api
{
    public class ApplicationSettings
    {
        public string MongoDbConnectionString { get; set; }
        public string MongoDbDatabase { get; set; }
    }
}
