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
        public string Comment { get; set; }
        public string Rating { get; set; }
        public string WatchedAt { get; set; }
        public string ReleasedAt { get; set; }

        public List<string> Genres { get; set; }
    }
}
