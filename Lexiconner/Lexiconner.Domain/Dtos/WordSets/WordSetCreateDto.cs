using Lexiconner.Domain.Dtos.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.WordSets
{
    public class WordSetCreateDto
    {
        public WordSetCreateDto()
        {
            Images = new List<GeneralImageDto>();
            Words = new List<WordSetCreateWordDto>();
        }

        public string Name { get; set; }
        public string WordsLanguageCode { get; set; }
        public string MeaningsLanguageCode { get; set; }
        public bool IsPublished { get; set; }
        public List<GeneralImageDto> Images { get; set; }
        public List<WordSetCreateWordDto> Words { get; set; }
    }

    public class WordSetCreateWordDto
    {
        public WordSetCreateWordDto()
        {
            Images = new List<GeneralImageDto>();
        }

        public string Word { get; set; }
        public string Meaning { get; set; }
        public List<string> Examples { get; set; }
        public List<GeneralImageDto> Images { get; set; }
    }
}
