using Lexiconner.Domain.Dtos.CustomCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dto.CustomCollections
{
    public class CustomCollectionsAllResponseDto
    {
        public CustomCollectionDto AsTree { get; set; }
        public IEnumerable<CustomCollectionDto> AsList { get; set; }
    }
}
