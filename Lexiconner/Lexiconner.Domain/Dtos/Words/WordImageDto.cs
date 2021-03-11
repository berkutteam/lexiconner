using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordImageDto
    {
        public WordImageDto()
        {
            RandomId = Guid.NewGuid().ToString();
        }

        public string RandomId { get; set; }
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Thumbnail { get; set; }
        public int? ThumbnailHeight { get; set; }
        public int? ThumbnailWidth { get; set; }
        public string Base64Encoding { get; set; }
    }
}
