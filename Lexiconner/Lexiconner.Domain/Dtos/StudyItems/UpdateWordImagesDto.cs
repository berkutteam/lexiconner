using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.StudyItems
{
    public class UpdateWordImagesDto
    {
        public UpdateWordImagesDto()
        {
            Images = new List<StudyItemImageUpdateDto>();
        }

        public IEnumerable<StudyItemImageUpdateDto> Images { get; set; }
    }

    public class StudyItemImageUpdateDto : StudyItemImageDto
    {
        public bool IsAddedByUrl { get; set; }
    }
}
