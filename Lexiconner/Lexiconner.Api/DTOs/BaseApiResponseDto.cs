using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs
{
    public class BaseApiResponseDto<T>
    {
        public bool Ok { get; set; }
        public T Data { get; set; }
    }
}
