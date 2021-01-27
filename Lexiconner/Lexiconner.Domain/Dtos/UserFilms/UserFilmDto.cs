using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.UserFilms
{
    public class UserFilmDto
    {
        public UserFilmDto()
        {
            Genres = new List<string>();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public decimal? MyRating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset? WatchedAt { get; set; }
        public int? ReleaseYear { get; set; }
        public List<string> Genres { get; set; }
        public string LanguageCode { get; set; }
        public UserFilmDetailsEntity Details { get; set; }
    }
}
