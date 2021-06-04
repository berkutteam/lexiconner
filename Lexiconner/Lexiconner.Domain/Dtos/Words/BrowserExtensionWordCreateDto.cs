using System.Collections.Generic;

namespace Lexiconner.Domain.Dtos.Words
{
    public class BrowserExtensionWordCreateDto
    {
        public BrowserExtensionWordCreateDto()
        {
            Examples = new List<string>();
        }

        public string Word { get; set; }
        public string Meaning { get; set; }
        public List<string> Examples { get; set; }
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }
    }
}
