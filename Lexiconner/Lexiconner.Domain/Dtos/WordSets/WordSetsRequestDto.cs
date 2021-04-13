using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.WordSets
{
    public class WordSetsRequestDto : PaginationRequestDto
    {
        public string LanguageCode { get; set; }
        public string Search { get; set; }
    }
}
