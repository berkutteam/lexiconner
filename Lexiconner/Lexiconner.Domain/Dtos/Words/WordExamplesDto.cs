using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordExamplesDto
    {
        public WordExamplesDto()
        {
            Examples = new List<WordExampleItemDto>();
        }

        public string Word { get; set; }
        public string LanguageCode { get; set; }
        public IEnumerable<WordExampleItemDto> Examples { get; set; }
    }

    public class WordExampleItemDto
    {
        public WordExampleItemDto()
        {
            RandomId = Guid.NewGuid().ToString();
        }

        public string RandomId { get; set; }
        public string Example { get; set; }
    }
}
