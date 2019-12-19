using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.StudyItems
{
    public class StudyItemsRequestDto : PaginationRequestDto
    {
        public string Search { get; set; }
        public bool? IsFavourite { get; set; }
    }
}
