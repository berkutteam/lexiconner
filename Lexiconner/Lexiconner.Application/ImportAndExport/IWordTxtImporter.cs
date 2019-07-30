using Lexiconner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Application.ImportAndExport
{
    public interface IWordTxtImporter
    {
        string SourceLanguageCode { get; }
        Task<IEnumerable<WordImportModel>> Import();
    }
}
