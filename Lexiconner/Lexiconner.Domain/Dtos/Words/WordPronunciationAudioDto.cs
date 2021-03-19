using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordPronunciationAudioDto
    {
        public string Word { get; set; }
        public string LanguageCode { get; set; }
        public string AudioMp3Url { get; set; }
        public string AudioOggUrl { get; set; }
    }
}
