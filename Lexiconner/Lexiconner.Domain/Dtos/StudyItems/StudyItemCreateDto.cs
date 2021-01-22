using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.StudyItems
{
    public class StudyItemCreateDto
    {
        public StudyItemCreateDto()
        {
            CustomCollectionIds = new List<string>();
            ExampleTexts = new List<string>();
            Tags = new List<string>();
        }

        public List<string> CustomCollectionIds { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> ExampleTexts { get; set; }
        public bool IsFavourite { get; set; }
        public string LanguageCode { get; set; }
        public List<string> Tags { get; set; }
    }
}
