using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Lexiconner.Application.Config;
using Lexiconner.Domain.Models;


namespace Lexiconner.Application.ImportAndExport
{
    
   public class FilmTxtImporter: IFilmTxtImporter
    {
        private readonly ImportSettings _config;
        public FilmTxtImporter(ImportSettings config)
        {
            _config = config;
        }

        public string SourceLanguageCode => "ru";

        public async Task<IEnumerable<FilmImportModel>> Import()
        {
            //(?:(?<Title>[\D]+?\d?)\s*(?<ReleasedAt>\d{0,4})\s*(?<Genre>[\D{0,4}]+))\s*===\s*(?<Rating>\d{0,2}[.\d]{0,3})?\s*(?<WatchedAt>\d{0,2}\.\d{0,2}\.\d{0,4})?
            return null;
        }

       
    }
  
}
