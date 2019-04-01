using Lexiconner.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.ImportAndExport
{
    public interface IWordTxtImporter
    {
        Task<IEnumerable<WordImportModel>> Import();
    }
}
