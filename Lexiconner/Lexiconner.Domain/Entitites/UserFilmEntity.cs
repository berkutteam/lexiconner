using Lexiconner.Domain.Dtos.UserFilms;
using Lexiconner.Domain.Entitites.Base;
using Lexiconner.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexiconner.Domain.Entitites
{
    public class UserFilmEntity : BaseEntity
    {
        public UserFilmEntity()
        {
            Genres = new List<string>();
        }

        public string UserId { get; set; }
        public string Title { get; set; }
        public decimal? MyRating { get; set; }
        public string Comment { get; set; }
        public DateTime? WatchedAt { get; set; }
        public int? ReleaseYear { get; set; }
        public List<string> Genres { get; set; }

        /// <summary>
        /// ISO 639-1 two-letter code.
        /// https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// https://developers.google.com/admin-sdk/directory/v1/languages
        /// </summary>
        public string LanguageCode { get; set; }

        public UserFilmDetailsEntity Details { get; set; }


        #region Helper methods

        public void UpdateSelf(UserFilmUpdateDto updateDto)
        {
            this.Title = updateDto.Title;
            this.MyRating = updateDto.MyRating;
            this.Comment = updateDto.Comment;
            this.WatchedAt = updateDto.WatchedAt;
            this.ReleaseYear = updateDto.ReleaseYear;
            this.Genres = updateDto.Genres;
            this.LanguageCode = updateDto.LanguageCode;
        }

        #endregion
    }

    /// <summary>
    /// Public general info (e.g. from TMDB).
    /// Language corresponds to that user selected for UserFilmEntity
    /// </summary>
    public class UserFilmDetailsEntity : BaseEntity
    {
        public UserFilmDetailsEntity()
        {
            Genres = new List<FilmGenreEntity>();
            ProductionCountries = new List<FilmProductionCountryEntity>();
        }

        public int TMDbId { get; set; }
        public string IMDbId { get; set; }
        public bool IsAdult { get; set; }
        public long Budget { get; set; }
        public List<FilmGenreEntity> Genres { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public List<FilmProductionCountryEntity> ProductionCountries { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public UserFilmImageEntity Image { get; set; }
    }

    public class FilmGenreEntity
    {
        public int TMDbGenreId { get; set; }
        public string Name { get; set; }
    }

    public class FilmProductionCountryEntity
    {
        public string Iso_3166_1 { get; set; }
        public string Name { get; set; }
    }

    public class UserFilmImageEntity
    {
        public string PosterUrl { get; set; }
        public int PosterWidth { get; set; }
        public string BackdropUrl { get; set; }
        public int BackdropWidth { get; set; }
        public string ThumbnailUrl { get; set; }
        public int ThumbnailWidth { get; set; }
    }

}