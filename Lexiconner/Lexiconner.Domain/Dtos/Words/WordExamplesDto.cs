using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordExamplesDto
    {
        public WordExamplesDto()
        {
            Examples = new List<string>();
        }

        public string Word { get; set; }
        public string LanguageCode { get; set; }
        public IEnumerable<string> Examples { get; set; }
    }
}
