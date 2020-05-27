using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Models
{
    public class StudyItemsSearchFilterModel
    {
        public string Search { get; set; }
        public bool? IsFavourite { get; set; }

        public StudyItemsSearchFilterModel(
            string search = null,
            bool? isFavourite = null
        )
        {
            Search = search;
            IsFavourite = isFavourite;
        }
    }
}
