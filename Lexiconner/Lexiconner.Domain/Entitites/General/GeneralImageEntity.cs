using Lexiconner.Domain.Entitites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Entitites.General
{
    public class GeneralImageEntity : BaseEntity
    {
        public bool IsAddedByUrl { get; set; }
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Thumbnail { get; set; }
        public int? ThumbnailHeight { get; set; }
        public int? ThumbnailWidth { get; set; }
        public string Base64Encoding { get; set; }
    }
}
