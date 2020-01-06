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
            List<FilmImportModel> result = new List<FilmImportModel>();
            string path = _config.FilmsFilePath;

            Regex CommentRegEx = new Regex(@"\s+(?:\/\*(?<Comment>.+?)\*\/)+\s+", RegexOptions.IgnoreCase);
            Regex RatingRegEx = new Regex(@"\s+(?<Rating>\d\d?\.\d)\s+", RegexOptions.IgnoreCase);
            Regex WatchedAtRegEx = new Regex(@"(?<WatchedAt>\d{0,2}\.\d{0,2}\.\d{0,4})", RegexOptions.IgnoreCase);
            Regex ReleasedAtRegEx = new Regex(@"(?:\s+(?<ReleasedAt>\d{4,})\s+)", RegexOptions.IgnoreCase);
            Regex GenresRegEx = new Regex(@"\s+(?:\((?<Genres>.+?)\))+\s+", RegexOptions.IgnoreCase);

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    List<string> genres = new List<string>();
                    string watchedAt = "";
                    string rating = "";
                    string comment = "";
                    string releasedAt = "";
                    string title = "";

                    string line = await reader.ReadLineAsync();

                    if (GenresRegEx.IsMatch(line))
                    {
                        var genresMatchStr = GenresRegEx.Match(line).Groups.Skip(1).First().Value;

                        line = line.Replace(genresMatchStr, "");

                        for (int i = 0; i < genresMatchStr.Split(',').Length; i++)
                        {
                           genres.Add(genresMatchStr.Split(',')[i]);
                        }

                    }

                    if (WatchedAtRegEx.IsMatch(line))
                    {
                        watchedAt = WatchedAtRegEx.Match(line).Groups.Skip(1).First().Value;

                        line = line.Replace(watchedAt, "");
                    }

                    if (RatingRegEx.IsMatch(line))
                    {
                         rating = RatingRegEx.Match(line).Groups.Skip(1).First().Value;

                         line = line.Replace(rating, "");
                    }

                    if (CommentRegEx.IsMatch(line))
                    {
                        comment = CommentRegEx.Match(line).Groups.Skip(1).First().Value;

                        line = line.Replace(comment, "");
                    }

                    if (ReleasedAtRegEx.IsMatch(line))
                    {
                        var releasedAtMatchStr = ReleasedAtRegEx.Match(line).Groups.Skip(1).First().Value;

                        line = line.Replace(releasedAtMatchStr, "");

                        line = line?.Replace(" ", "");

                        if (line == "") // if the film name is just a date
                        {
                           title = releasedAtMatchStr;
                        }
                        else
                        {
                            title = line;
                            releasedAt = releasedAtMatchStr;
                        }
                    }
                    else
                    {
                        title = line;
                    }

                    result.Add(new FilmImportModel()
                    {
                        Title = title ?? null,
                        Comment = comment ?? null,
                        Rating = rating ?? null,
                        WatchedAt = watchedAt ?? null,
                        ReleasedAt = releasedAt ?? null,
                        Genres = genres.ToList()?? null

                    });

                }
            }

            return result;

        }

    }

}
