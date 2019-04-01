using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Models
{
    public class BaseApiResponseModel<T>
    {
        public T Data { get; set; }
    }
}
