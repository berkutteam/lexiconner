using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Models
{
    public class FilmImportModel
    {
        public FilmImportModel()
        {
            Tags = new List<string>();
        }

        public string Title { get; set; }
        public double Rating { get; set; }

        public DateTimeOffset WatchedAt { get; set; }
        public List<string> Tags { get; set; }
    }
}
