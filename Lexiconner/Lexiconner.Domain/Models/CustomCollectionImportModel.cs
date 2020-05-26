using Lexiconner.Domain.Interfaces;
using NUlid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Models
{
    public class CustomCollectionImportModel : ITraversable<CustomCollectionImportModel>
    {
        public CustomCollectionImportModel()
        {
            TempId = Ulid.NewUlid(new NUlid.Rng.CSUlidRng()).ToString();
            Children = new List<CustomCollectionImportModel>();
        }

        public string TempId { get; set; }
        public string Name { get; set; }

        public CustomCollectionImportModel Current { get { return this; } }
        public List<CustomCollectionImportModel> Children { get; set; }
    }
}
