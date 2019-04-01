using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Models.ResponseModels
{
    public class GetAllResponseModel<T>
    {
        public long TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
