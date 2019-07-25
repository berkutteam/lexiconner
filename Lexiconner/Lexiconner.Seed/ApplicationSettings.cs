using Lexiconner.Application.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Seed
{
    public class ApplicationSettings
    {
        public ImportSettings Import { get; set; }
        public MongoDbSettings MongoDb { get; set; }
        public GoogleSettings Google { get; set; }
        public RapidApiSettings RapidApi { get; set; }
    }

    public class ImportSettings
    {
        public string FilePath { get; set; }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string DatabaseIdentity { get; set; }
    }
}
