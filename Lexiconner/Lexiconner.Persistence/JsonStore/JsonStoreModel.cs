using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Persistence.JsonStore
{
    public class JsonStoreModel
    {
        public JsonStoreModel()
        {
            Words = new List<WordEntity>();
        }

        public List<WordEntity> Words { get; set; }
    }
}
