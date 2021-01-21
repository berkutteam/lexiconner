using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Lexiconner.Domain.Models;
using System.Globalization;
using Lexiconner.Application.ApplicationSettings;

namespace Lexiconner.Application.ImportAndExport
{
    
   public class FilmImporter: IFilmImporter
    {
        private readonly ImportSettings _config;
        public FilmImporter(ImportSettings config)
        {
            _config = config;
        }

        public async Task<IEnumerable<FilmImportModel>> ImportTxtFormatFilmsAsync(string filePath)
        {
            List<FilmImportModel> result = new List<FilmImportModel>();

            // <Cool film 2: Trip in 2018>[title] <(Comedy, Detective)>[Genres] <2019>[releasedAt] <8.0>[rating] </*some comment*/>[comment] <24.12.18>[watchedAt]
            Regex titleRegEx = new Regex(@"\s{0,}(?<title>[^=]+)\s+(?:===)\s{0,}", RegexOptions.IgnoreCase);
            Regex genresRegEx = new Regex(@"\s+\((?<genres>(?:\s{0,},?[^\s,)]+\s{0,})+)\)\s{0,}", RegexOptions.IgnoreCase);
            Regex releaseYearRegEx = new Regex(@"\s+(?<releaseYear>\d{4})\s{0,}", RegexOptions.IgnoreCase);
            Regex ratingRegEx = new Regex(@"\s+(?<rating>\d{1,2}\.\d{1,2})\s{0,}", RegexOptions.IgnoreCase);
            Regex commentRegEx = new Regex(@"\s+/\*(?<comment>.+)\*/\s{0,}", RegexOptions.IgnoreCase);
            Regex watchedAtRegEx = new Regex(@"\s+(?<watchedAt>\d{2}\.\d{2}\.\d{2})\s{0,}", RegexOptions.IgnoreCase);

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string title = "";
                    List<string> genres = new List<string>();
                    int? releaseYear = null;
                    decimal? myRating = null;
                    string comment = "";
                    DateTime? watchedAt = null;

                    string line = await reader.ReadLineAsync();

                    if (titleRegEx.IsMatch(line))
                    {
                        title = titleRegEx.Match(line).Groups.Values.FirstOrDefault(x => x.Name == "title")?.Value.Trim();
                    }
                    if (genresRegEx.IsMatch(line))
                    {
                        string genresRaw = genresRegEx.Match(line).Groups.Values.FirstOrDefault(x => x.Name == "genres")?.Value.Trim();
                        genres = genresRaw.Split(',').Select(x => x.Trim()).ToList();
                    }
                    if (releaseYearRegEx.IsMatch(line))
                    {
                        string releaseYearRaw = releaseYearRegEx.Match(line).Groups.Values.FirstOrDefault(x => x.Name == "releaseYear")?.Value.Trim();
                        if (!String.IsNullOrEmpty(releaseYearRaw))
                        {
                            releaseYear = int.Parse(releaseYearRaw);
                        }
                    }
                    if (ratingRegEx.IsMatch(line))
                    {
                        string ratingRaw = ratingRegEx.Match(line).Groups.Values.FirstOrDefault(x => x.Name == "rating")?.Value.Trim();
                        if (!String.IsNullOrEmpty(ratingRaw))
                        {
                            myRating = decimal.Parse(ratingRaw);
                        }
                    }
                    if (commentRegEx.IsMatch(line))
                    {
                        comment = commentRegEx.Match(line).Groups.Values.FirstOrDefault(x => x.Name == "comment")?.Value.Trim();
                    }
                    if (watchedAtRegEx.IsMatch(line))
                    {
                        string watchedAtRaw = watchedAtRegEx.Match(line).Groups.Values.FirstOrDefault(x => x.Name == "watchedAt")?.Value.Trim();
                        if (!String.IsNullOrEmpty(watchedAtRaw))
                        {
                            watchedAt = DateTime.ParseExact(watchedAtRaw, "dd.MM.y", CultureInfo.InvariantCulture);
                        }
                    }

                    result.Add(new FilmImportModel()
                    {
                        Title = title,
                        Genres = genres,
                        ReleaseYear = releaseYear,
                        MyRating = myRating,
                        Comment = comment,
                        WatchedAt = watchedAt,
                    });
                }
            }

            return result;
        }
    }
}
