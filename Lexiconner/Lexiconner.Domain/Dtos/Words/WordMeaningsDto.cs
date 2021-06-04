using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordMeaningsDto
    {
        public WordMeaningsDto()
        {
            Meanings = new List<WordMeaningsItemDto>();
        }

        public string Word { get; set; }
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }
        public IEnumerable<WordMeaningsItemDto> Meanings { get; set; }
    }

    public class WordMeaningsItemDto
    {
        public WordMeaningsItemDto()
        {
            RandomId = Guid.NewGuid().ToString();
        }

        public string RandomId { get; set; }
        public string Meaning { get; set; }
        public string PartOfSpeechTag { get; set; }
    }
}
