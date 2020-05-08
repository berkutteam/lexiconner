using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.DTOs.CustomCollections
{
    public class CustomCollectionCreateDto
    {
        public string ParentCollectionId { get; set; }
        public string Name { get; set; }
    }
}
