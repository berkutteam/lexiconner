using FluentValidation;
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

        public CustomCollectionImportModel AddCollection(string name, string parentName = null)
        {
            CustomCollectionImportModel addedCollection = null;
            var flatten = this.Collections.SelectMany(x => x.Flatten()).ToList();

            // validate name is unique
            if(flatten.Any(x => x.Name == name))
            {
                throw new ValidationException("All collection names must be unique!");
            }

            var parent = flatten.FirstOrDefault(x => x.Name == parentName);
            if(parent != null)
            {
                var existing = parent.Children.FirstOrDefault(x => x.Name == name);
                if(existing == null)
                {
                    addedCollection = new CustomCollectionImportModel()
                    {
                        Name = name,
                    };
                    parent.Children.Add(addedCollection);
                }
            }
            else
            {
                var existing = flatten.FirstOrDefault(x => x.Name == name);
                if (existing == null)
                {
                    addedCollection = new CustomCollectionImportModel()
                    {
                        Name = name,
                    };
                    this.Collections.Add(addedCollection);
                }
            }
            return addedCollection;
        }
    }
}
