using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.ApiClients.Dtos.OxfordLearnersDictionaries
{
    public class OxfordPronunciationAudioScraperResultDto
    {
        public OxfordPronunciationAudioScraperResultDto()
        {
            Results = new List<OxfordPronunciationAudioScraperResultItemDto>();
        }

        /// <summary>
        /// E.g. english
        /// </summary>
        public string SourceLanguage { get; set; }

        /// <summary>
        /// E.g. en
        /// </summary>
        public string WordLanguageCode { get; set; }
        public string TargetLanguage { get; set; }
        public string MeaningLanguageCode { get; set; }
        public string SearchedWord { get; set; }
        public IList<OxfordPronunciationAudioScraperResultItemDto> Results { get; set; }
    }

    public class OxfordPronunciationAudioScraperResultItemDto
    {
        public string UKAudioMp3Url { get; set; }
        public string UKAudioOggUrl { get; set; }
        public string USAudioMp3Url { get; set; }
        public string USAudioOggUrl { get; set; }
    }
}
