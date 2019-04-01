using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Lexiconner.Api.Models;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Lexiconner.Api.ImportAndExport
{
    public class WordTxtImporter : IWordTxtImporter
    {
        private readonly IConfiguration _configuration;

        public WordTxtImporter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<WordImportModel>> Import()
        {
            var result = new List<WordImportModel>();
            string filePath = _configuration.GetValue<string>("Import:FilePath");

            // word - desc[ - example]
            var regex = new Regex(@"([^-]+)\s?-\s?([^-]+)\s?-?\s?([^-]+)?", RegexOptions.IgnoreCase);
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
                        Word = parts[0],
                        Description = parts[1],
                        ExampleText = parts.Count > 2 ? parts[2] : String.Empty,
                        Tags = currentTags.ToList()
                    });
                }
            }

            return result;
        }
    }
}
