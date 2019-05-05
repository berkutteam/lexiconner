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
            StudyItems = new List<StudyItemEntity>();
        }

        public List<StudyItemEntity> StudyItems { get; set; }
    }
}
