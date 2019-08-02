using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Persistence.UnitTests
{
    public class ApplicationSettings
    {
        public MongoDbSettings MongoDb { get; set; }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string DatabaseIdentity { get; set; }
    }
}
