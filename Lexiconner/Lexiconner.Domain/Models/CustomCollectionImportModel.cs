using Lexiconner.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Models
{
    public class CustomCollectionImportModel : ITraversable<CustomCollectionImportModel>
    {
        public CustomCollectionImportModel()
        {
            Children = new List<CustomCollectionImportModel>();
        }

        public string Name { get; set; }

        public CustomCollectionImportModel Current { get { return this; } }
        public List<CustomCollectionImportModel> Children { get; set; }
    }
}
