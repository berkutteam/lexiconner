using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs
{
    public class PaginationResponseDto<T>
    {
        public long TotalCount { get; set; }
        public int ReturnedCount { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
