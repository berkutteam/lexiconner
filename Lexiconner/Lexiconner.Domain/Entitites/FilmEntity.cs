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
    class FilmEntity : BaseEntity
    {
        public FilmEntity()
        {
            Genre = new List<string>();
        }


        public string UserId { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public double Rating { get; set; }
        public DateTime WatchedAt { get; set; }
        public DateTime ReleasedAt { get; set; }
        public FilmItemImageEntity Image { get; set; }

        public List<string> Genre { get; set; }
    }

    public class FilmItemImageEntity : BaseEntity
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