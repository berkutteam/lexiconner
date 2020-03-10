using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.StudyItems
{
    public class StudyItemDto
    {
        public StudyItemDto()
        {
            Tags = new List<string>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ExampleText { get; set; }
        public bool IsFavourite { get; set; }
        public string LanguageCode { get; set; }
        public List<string> Tags { get; set; }

        public StudyItemImageEntity Image { get; set; }
    }
}
