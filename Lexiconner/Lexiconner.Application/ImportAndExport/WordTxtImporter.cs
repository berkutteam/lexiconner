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

        public async Task<WordImportResultModel> ImportTxtFormatWords(string filePath)
        {
            // TODO: handle collections

            var result = new WordImportResultModel();

            // word === desc[ === example]
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
                        var tag = tagRegex.Match(line).Groups.Values.Skip(1).First().Value;
                        currentTags.Add(tag);
                        continue;
                    }
                    else if (endTagRegex.IsMatch(line))
                    {
                        currentTags.Clear();
                        continue;
                    }

                    var match = regex.Match(line);
                    var parts = match.Groups.Values.Skip(1).Select(x => x.Value).ToList();

                    if (parts.Count < 2)
                    {
                        throw new InvalidOperationException($"Import data was in incorrect format.");
                    }

                    result.Words.Add(new WordImportModel
                    {
                        Title = match.Groups.Values.FirstOrDefault(x => x.Name == "word")?.Value,
                        Description = match.Groups.Values.FirstOrDefault(x => x.Name == "description")?.Value,
                        ExampleTexts = new List<string>() 
                        {
                            match.Groups.Values.FirstOrDefault(x => x.Name == "example")?.Value
                        },
                        Tags = currentTags.ToList()
                    });
                }
            }

            return result;
        }

        public async Task<WordImportResultModel> ImportMdFormatWords(string filePath)
        {
            var result = new WordImportResultModel();

            // #Header1
            // ##Header2
            // ###Header3
            // ####Header4
            // *** (3 asterisks)
            // Title
            // Description
            // Example 1
            // Example 2
            // ...
            // Example N
            // --- (3 hyphens)
            // ...

            var commentStartRegex = new Regex(@"^\s*(<!--|<!---)\s*$", RegexOptions.IgnoreCase);
            var commentEndRegex = new Regex(@"^\s*(-->|--->)\s*$", RegexOptions.IgnoreCase);

            var header1Regex = new Regex(@"^#\s*([^#]+)\s*$", RegexOptions.IgnoreCase);
            var header2Regex = new Regex(@"^##\s*([^#]+)\s*$", RegexOptions.IgnoreCase);
            var header3Regex = new Regex(@"^###\s*([^#]+)\s*$", RegexOptions.IgnoreCase);
            var header4Regex = new Regex(@"^####\s*([^#]+)\s*$", RegexOptions.IgnoreCase);
            var header5Regex = new Regex(@"^#####\s*([^#]+)\s*$", RegexOptions.IgnoreCase);
            var header6Regex = new Regex(@"^######\s*([^#]+)\s*$", RegexOptions.IgnoreCase);

            var wordStartRegex = new Regex(@"^\s*(\*\*\*)\s*$", RegexOptions.IgnoreCase);
            var wordEndRegex = new Regex(@"^\s*(---)\s*$", RegexOptions.IgnoreCase);

            bool isCommentEntered = false;
            string currentHeader1 = null;
            string currentHeader2 = null;
            string currentHeader3 = null;
            string currentHeader4 = null;
            string currentHeader5 = null;
            string currentHeader6 = null;

            CustomCollectionImportModel lastAddedCollection = null;
            bool isWordEntered = false;
            string currentTitle = null;
            string currentDescription = null;
            List<string> currentExamples = new List<string>();
            var resetCurrentWord = new Action(() =>
            {
                isWordEntered = false;
                currentTitle = null;
                currentDescription = null;
                currentExamples = new List<string>();
            });
            resetCurrentWord();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    line = line.Trim();

                    // empty line
                    if (String.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // comments
                    if (commentStartRegex.IsMatch(line))
                    {
                        isCommentEntered = true;
                        continue;
                    }
                    else if (commentEndRegex.IsMatch(line))
                    {
                        isCommentEntered = false;
                        continue;
                    }
                    else if(isCommentEntered)
                    {
                        // skip commented lines
                        continue;
                    }

                    // headers
                    else if (header1Regex.IsMatch(line))
                    {
                        currentHeader1 = header1Regex.Match(line).Groups.Values.Skip(1).First().Value;
                        currentHeader2 = null;
                        currentHeader3 = null;
                        currentHeader4 = null;
                        currentHeader5 = null;
                        currentHeader6 = null;

                        lastAddedCollection = result.AddCollection(currentHeader1, null);

                        continue;
                    }
                    else if (header2Regex.IsMatch(line))
                    {
                        currentHeader2 = header2Regex.Match(line).Groups.Values.Skip(1).First().Value;
                        currentHeader3 = null;
                        currentHeader4 = null;
                        currentHeader5 = null;
                        currentHeader6 = null;

                        lastAddedCollection = result.AddCollection(currentHeader2, currentHeader1);

                        continue;
                    }
                    else if (header3Regex.IsMatch(line))
                    {
                        currentHeader3 = header3Regex.Match(line).Groups.Values.Skip(1).First().Value;
                        currentHeader4 = null;
                        currentHeader5 = null;
                        currentHeader6 = null;

                        lastAddedCollection = result.AddCollection(currentHeader3, currentHeader2);

                        continue;
                    }
                    else if (header4Regex.IsMatch(line))
                    {
                        currentHeader4 = header4Regex.Match(line).Groups.Values.Skip(1).First().Value;
                        currentHeader5 = null;
                        currentHeader6 = null;

                        lastAddedCollection = result.AddCollection(currentHeader4, currentHeader3);

                        continue;
                    }
                    else if (header5Regex.IsMatch(line))
                    {
                        currentHeader5 = header5Regex.Match(line).Groups.Values.Skip(1).First().Value;
                        currentHeader6 = null;

                        lastAddedCollection = result.AddCollection(currentHeader5, currentHeader4);

                        continue;
                    }
                    else if (header6Regex.IsMatch(line))
                    {
                        currentHeader6 = header6Regex.Match(line).Groups.Values.Skip(1).First().Value;

                        lastAddedCollection = result.AddCollection(currentHeader6, currentHeader5);

                        continue;
                    }

                    // start of word
                    else if (wordStartRegex.IsMatch(line))
                    {
                        isWordEntered = true;
                        continue;
                    }
                    // end of word
                    else if (wordEndRegex.IsMatch(line))
                    {
                        isWordEntered = false;

                        // save word
                        result.Words.Add(new WordImportModel
                        {
                            CollectionTempId = lastAddedCollection?.TempId,
                            CollectionName = lastAddedCollection?.Name,
                            Title = currentTitle,
                            Description = currentDescription,
                            ExampleTexts = currentExamples,
                            Tags = new List<string>(),
                        });
                        resetCurrentWord();
                        continue;
                    }

                    // word fields
                    else if (isWordEntered)
                    {
                        if (String.IsNullOrEmpty(currentTitle))
                        {
                            currentTitle = line;
                            continue;
                        }
                        else if (String.IsNullOrEmpty(currentDescription))
                        {
                            currentDescription = line;
                            continue;
                        }

                        currentExamples.Add(line);
                        continue;
                    }
                }
            }

            return result;
        }
    }
}
