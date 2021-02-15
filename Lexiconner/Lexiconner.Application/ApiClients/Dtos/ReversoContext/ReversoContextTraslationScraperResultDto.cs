using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApiClients.Dtos.ReversoContext
{
    public class ReversoContextTraslationScraperResultDto
    {
        public ReversoContextTraslationScraperResultDto()
        {
            Results = new List<ReversoContextTraslationScraperResultItemDto>();
        }

        /// <summary>
        /// E.g. english
        /// </summary>
        public string SourceLanguage { get; set; }

        /// <summary>
        /// E.g. en
        /// </summary>
        public string SourceLanguageCode { get; set; }
        public string TargetLanguage { get; set; }
        public string TargetLanguageCode { get; set; }
        public string SearchedWord { get; set; }
        public IList<ReversoContextTraslationScraperResultItemDto> Results { get; set; }
    }

    public class ReversoContextTraslationScraperResultItemDto
    {
        public string SourceLanguageSentence { get; set; }
        public string TargetLanguageSentence { get; set; }
    }
}
