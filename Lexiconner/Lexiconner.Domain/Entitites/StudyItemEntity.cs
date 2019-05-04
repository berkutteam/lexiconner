using Lexiconner.Domain.Entitites.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Entitites
{
    public class StudyItemEntity : BaseEntity
    {
        public StudyItemEntity()
        {
            Tags = new List<string>();
        }

        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ExampleText { get; set; }
        public string ExampleImageUrl { get; set; }

        public List<string> Tags { get; set; }
    }
}
