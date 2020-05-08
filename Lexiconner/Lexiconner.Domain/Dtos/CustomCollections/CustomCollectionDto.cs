using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.DTOs.CustomCollections
{
    public class CustomCollectionDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
        public List<CustomCollectionDto> Children { get; set; }

        public IEnumerable<CustomCollectionDto> FlattenToList()
        {
            var currentResult = new List<CustomCollectionDto>()
            {
                this,
            };
            if (!this.Children.Any())
            {
                return currentResult;
            }
            else
            {
                var childrenResult = this.Children.SelectMany(x => x.FlattenToList()).ToList();
                currentResult.AddRange(childrenResult);
                return currentResult;
            }
        }
    }
}
