using Lexiconner.Domain.Interfaces;
using MongoDB.Bson;
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
            TempId = ObjectId.GenerateNewId().ToString();
            Children = new List<CustomCollectionImportModel>();
            ImageUrls = new List<string>();
        }

        public string TempId { get; set; }
        public string Name { get; set; }

        public CustomCollectionImportModel Current { get { return this; } }
        public List<CustomCollectionImportModel> Children { get; set; }

        public List<string> ImageUrls { get; set; }
    }
}
