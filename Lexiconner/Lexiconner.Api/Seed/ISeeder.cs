using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Seed
{
    public interface ISeeder
    {
        Task Seed();
    }
}
