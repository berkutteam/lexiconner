using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordsRequestDto : PaginationRequestDto
    {
        public string LanguageCode { get; set; }
        public string Search { get; set; }
        public bool? IsFavourite { get; set; }
        public bool IsShuffle { get; set; }
        public bool? IsTrained { get; set; }
        public string CollectionId { get; set; }
        public string UserWordSetId { get; set; }
    }
}
