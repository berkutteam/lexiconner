using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Models
{
    public class WordImportModel
    {
        public WordImportModel()
        {
            ExampleTexts = new List<string>();
            Tags = new List<string>();
        }

        public string CollectionTempId { get; set; }
        public string CollectionName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> ExampleTexts { get; set; }
        public List<string> Tags { get; set; }
    }
}
