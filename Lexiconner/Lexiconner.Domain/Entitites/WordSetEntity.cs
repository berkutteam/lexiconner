using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites.Base;
using Lexiconner.Domain.Entitites.General;
using Lexiconner.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexiconner.Domain.Entitites
{
    public class WordSetEntity : BaseEntity
    {
        public WordSetEntity()
        {
            Images = new List<GeneralImageEntity>();
            Words = new List<WordSetWordEntity>();
        }

        public string Name { get; set; }
        public string WordsLanguageCode { get; set; }
        public string MeaningsLanguageCode { get; set; }
        public List<GeneralImageEntity> Images { get; set; }
        public List<WordSetWordEntity> Words { get; set; }
    }

    public class WordSetWordEntity : WordGeneralEntity
    {
    }
}
