using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Models
{
    public class FilmImportModel
    {
        public FilmImportModel()
        {
            Genres = new List<string>();
        }

        public string Title { get; set; }
        public decimal? MyRating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset? WatchedAt { get; set; }
        public int? ReleaseYear { get; set; }
        public List<string> Genres { get; set; }
    }
}
