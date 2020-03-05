using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.StudyItems
{
    public class StudyItemCreateDto
    {
        public StudyItemCreateDto()
        {
            Tags = new List<string>();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ExampleText { get; set; }
        public bool IsFavourite { get; set; }
        public string LanguageCode { get; set; }
        public List<string> Tags { get; set; }
    }
}
