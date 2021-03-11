using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordUpdateDto
    {
        public WordUpdateDto()
        {
            CustomCollectionIds = new List<string>();
            Examples = new List<string>();
            Tags = new List<string>();
        }

        public List<string> CustomCollectionIds { get; set; }
        public string Word { get; set; }
        public string Meaning { get; set; }
        public List<string> Examples { get; set; }
        public bool IsFavourite { get; set; }
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }
        public List<string> Tags { get; set; }
    }
}
