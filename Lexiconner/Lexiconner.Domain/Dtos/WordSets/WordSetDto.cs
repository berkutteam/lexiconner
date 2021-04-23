using Lexiconner.Domain.Dtos.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.WordSets
{
    public class WordSetDto
    {
        public WordSetDto()
        {
            Images = new List<GeneralImageDto>();
            Words = new List<WordSetWordDto>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string WordsLanguageCode { get; set; }
        public string MeaningsLanguageCode { get; set; }
        public List<GeneralImageDto> Images { get; set; }
        public List<WordSetWordDto> Words { get; set; }
    }

    public class WordSetWordDto
    {
        public WordSetWordDto()
        {
            Images = new List<GeneralImageDto>();
        }

        public string Id { get; set; }
        public string Word { get; set; }
        public string Meaning { get; set; }
        public List<string> Examples { get; set; }
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }
        public List<GeneralImageDto> Images { get; set; }
    }
}
