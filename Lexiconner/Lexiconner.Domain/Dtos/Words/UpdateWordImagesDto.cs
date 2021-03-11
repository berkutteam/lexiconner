using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words
{
    public class UpdateWordImagesDto
    {
        public UpdateWordImagesDto()
        {
            Images = new List<WordImageUpdateDto>();
        }

        public IEnumerable<WordImageUpdateDto> Images { get; set; }
    }

    public class WordImageUpdateDto : WordImageDto
    {
        public bool IsAddedByUrl { get; set; }
    }
}
