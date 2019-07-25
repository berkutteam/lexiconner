using Lexiconner.Seed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Seed.Seed.ImportAndExport
{
    public interface IWordTxtImporter
    {
        string SourceLanguageCode { get; }
        Task<IEnumerable<WordImportModel>> Import();
    }
}
