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

        public UserFilmImageEntity Image { get; set; }


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

    public class UserFilmImageEntity : BaseEntity
    {
        public string Url { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailHeight { get; set; }
        public string ThumbnailWidth { get; set; }
        public string Base64Encoding { get; set; }
    }

}