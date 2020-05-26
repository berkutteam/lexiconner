using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.StudyItems
{
    public class StudyItemUpdateDto
    {
        public StudyItemUpdateDto()
        {
            ExampleTexts = new List<string>();
            Tags = new List<string>();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> ExampleTexts { get; set; }
        public bool IsFavourite { get; set; }
        public string LanguageCode { get; set; }
        public List<string> Tags { get; set; }
    }
}
