using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Models.RequestModels
{
    public class GetAllRequestModel
    {
        public string Search { get; set; }
        public int? Offset { get; set; } = 0;
        public int? Limit { get; set; } = 10;
    }
}
