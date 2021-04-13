using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.General
{
    public class GeneralImageDto
    {
        public GeneralImageDto()
        {
            RandomId = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
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
