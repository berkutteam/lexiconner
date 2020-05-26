using Lexiconner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Application.ImportAndExport
{
    public interface IWordTxtImporter
    {
        Task<WordImportResultModel> ImportTxtFormatWords(string filePath);
        Task<WordImportResultModel> ImportMdFormatWords(string filePath);
    }
}
