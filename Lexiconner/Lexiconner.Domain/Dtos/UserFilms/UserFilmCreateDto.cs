using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.UserFilms
{
    public class UserFilmCreateDto
    {
        public UserFilmCreateDto()
        {
            Genres = new List<string>();
        }

        public string Title { get; set; }
        public decimal? MyRating { get; set; }
        public DateTime? WatchedAt { get; set; }
        public int? ReleaseYear { get; set; }
        public List<string> Genres { get; set; }
        public string Comment { get; set; }
        public string LanguageCode { get; set; }
    }
}
