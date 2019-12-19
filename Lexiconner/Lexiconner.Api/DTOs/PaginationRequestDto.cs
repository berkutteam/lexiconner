using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs
{
    public class PaginationRequestDto
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
