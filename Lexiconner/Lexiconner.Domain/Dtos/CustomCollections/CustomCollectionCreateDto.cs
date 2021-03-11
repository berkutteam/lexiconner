using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.CustomCollections
{
    public class CustomCollectionCreateDto
    {
        public string ParentCollectionId { get; set; }
        public string Name { get; set; }
    }
}
