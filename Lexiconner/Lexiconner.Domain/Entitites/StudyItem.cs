using Lexiconner.Domain.Entitites.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites
{
    public class StudyItem : BaseEntity
    {
        public StudyItem()
        {
            Tags = new List<string>();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ExampleText { get; set; }
        public string ExampleImageUrl { get; set; }

        public List<string> Tags { get; set; }
    }
}
