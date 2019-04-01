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
            SudyItems = new List<StudyItem>();
        }

        public List<StudyItem> SudyItems { get; set; }
    }
}
