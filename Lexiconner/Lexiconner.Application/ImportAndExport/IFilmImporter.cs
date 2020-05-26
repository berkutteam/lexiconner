using Lexiconner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Application.ImportAndExport
{
    public interface IFilmImporter
    {
        Task<IEnumerable<FilmImportModel>> ImportTxtFormatFilmsAsync(string filePath);
    }
}