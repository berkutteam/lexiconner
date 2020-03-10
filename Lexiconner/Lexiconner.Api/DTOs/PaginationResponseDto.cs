using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs
{
    public class PaginationResponseDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PaginationInfoDto Pagination { get; set; }
    }

    public class PaginationInfoDto
    {
        public long TotalCount { get; set; }
        public int ReturnedCount { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
