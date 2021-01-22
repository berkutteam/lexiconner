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

        /// <summary>
        /// Level 0 is root
        /// </summary>
        public int Level { get; set; }
        public List<CustomCollectionDto> Children { get; set; }

        public IEnumerable<CustomCollectionDto> FlattenToList(int level = 0)
        {
            this.Level = level;
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
                var childrenResult = this.Children.SelectMany(x => x.FlattenToList(level + 1)).ToList();
                currentResult.AddRange(childrenResult);
                return currentResult;
            }
        }
    }
}
