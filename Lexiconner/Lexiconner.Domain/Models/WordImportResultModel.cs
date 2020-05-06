using Lexiconner.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lexiconner.Domain.Models
{
    public class WordImportResultModel
    {
        public WordImportResultModel()
        {
            Collections = new List<CustomCollectionImportModel>();
            Words = new List<WordImportModel>();    
        }

        public List<CustomCollectionImportModel> Collections { get; set; }
        public List<WordImportModel> Words { get; set; }

        public void AddCollection(string name, string parentName = null)
        {
            var flatten = this.Collections.SelectMany(x => x.Flatten()).ToList();
            var parent = flatten.FirstOrDefault(x => x.Name == parentName);
            if(parent != null)
            {
                var existing = parent.Children.FirstOrDefault(x => x.Name == name);
                if(existing == null)
                {
                    parent.Children.Add(new CustomCollectionImportModel()
                    {
                        Name = name,
                    });
                }
            }
            else
            {
                var existing = flatten.FirstOrDefault(x => x.Name == name);
                if (existing == null)
                {
                    this.Collections.Add(new CustomCollectionImportModel()
                    {
                        Name = name,
                    });
                }
            }
        }
    }
}
