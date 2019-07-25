using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Seed.Seed
{
    public class SeedServiceProductionHeroku : ISeedService
    {
        public Task RemoveDatabaseAsync()
        {
            throw new NotImplementedException();
        }

        public Task SeedAsync()
        {
            throw new NotImplementedException();
        }
    }
}
