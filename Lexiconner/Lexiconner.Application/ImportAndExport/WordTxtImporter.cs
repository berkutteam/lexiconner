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
    public class WordTxtImporter : IWordTxtImporter
    {
        private readonly ImportSettings _config;

        public WordTxtImporter(ImportSettings config)
        {
            _config = config;
        }

        public string SourceLanguageCode => "ru";

        public async Task<IEnumerable<WordImportModel>> Import()
        {
            var result = new List<WordImportModel>();
            string filePath = _config.FilePath;

            // word - desc[ - example]
            var regex = new Regex(@"(?<word>[^=]+)\s+===\s+(?<description>[^=]+)(?:\s{0,}(:?===)?\s{0,}(?<example>[^=]+)?)", RegexOptions.IgnoreCase);
            var tagRegex = new Regex(@"#([^\/]+)", RegexOptions.IgnoreCase);
            var endTagRegex = new Regex(@"#\/([^\/])*", RegexOptions.IgnoreCase);
            List<string> currentTags = new List<string>();

            using (var reader = new StreamReader(filePath))
            {
                while(!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    line = line.Trim();

                    if (String.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    if(tagRegex.IsMatch(line))
                    {
                        var tag = tagRegex.Match(line).Groups.Skip(1).First().Value;
                        currentTags.Add(tag);
                        continue;
                    }
                    else if (endTagRegex.IsMatch(line))
                    {
                        currentTags.Clear();
                        continue;
                    }

                    var match = regex.Match(line);
                    var parts = match.Groups.Skip(1).Select(x => x.Value).ToList();

                    if (parts.Count < 2)
                    {
                        throw new InvalidOperationException($"Import data was in incorrect format.");
                    }

                    result.Add(new WordImportModel
                    {
                        Word = match.Groups.FirstOrDefault(x => x.Name == "word")?.Value,
                        Description = match.Groups.FirstOrDefault(x => x.Name == "description")?.Value,
                        ExampleText = match.Groups.FirstOrDefault(x => x.Name == "example")?.Value,
                        Tags = currentTags.ToList()
                    });
                }
            }

            return result;
        }
    }
}
