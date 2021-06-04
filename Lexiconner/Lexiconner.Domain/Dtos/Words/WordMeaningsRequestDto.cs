using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordMeaningsRequestDto
    {
        public string Word { get; set; }
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }
    }
}
