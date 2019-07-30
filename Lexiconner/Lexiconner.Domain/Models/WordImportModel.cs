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
            Tags = new List<string>();
        }

        public string Word { get; set; }
        public string Description { get; set; }
        public string ExampleText { get; set; }
        public List<string> Tags { get; set; }
    }
}
