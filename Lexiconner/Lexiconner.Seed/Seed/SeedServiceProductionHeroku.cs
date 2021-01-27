using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Seed.Seed
{
    public class SeedServiceProductionHeroku : ISeedService
    {
        private readonly SeedServiceDevelopmentLocalhost _seedServiceDevelopmentLocalhost;

        public SeedServiceProductionHeroku(
            SeedServiceDevelopmentLocalhost seedServiceDevelopmentLocalhost
        )
        {
            _seedServiceDevelopmentLocalhost = seedServiceDevelopmentLocalhost;
        }

        public Task RemoveDatabaseAsync()
        {
            return _seedServiceDevelopmentLocalhost.RemoveDatabaseAsync();
        }

        public Task SeedAsync()
        {
            return _seedServiceDevelopmentLocalhost.SeedAsync();
        }
    }
}
