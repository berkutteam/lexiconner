using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Models
{
    public class UserFilmsSearchFilterModel
    {
        public string Search { get; set; }

        public UserFilmsSearchFilterModel(
            string search = null
        )
        {
            Search = search;
        }
    }
}
