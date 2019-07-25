using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Seed.Seed
{
    public interface ISeedService
    {
        Task SeedAsync();
        Task RemoveDatabaseAsync();
    }
}
