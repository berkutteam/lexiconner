using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordPronunciationAudioRequestDto
    {
        public string LanguageCode { get; set; }
        public string Word { get; set; }
    }
}
