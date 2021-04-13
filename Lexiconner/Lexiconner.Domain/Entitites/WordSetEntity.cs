using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites.Base;
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
            Images = new List<WordImageEntity>();
            Words = new List<WordSetWordEntity>();
        }

        public string Name { get; set; }
        public string WordsLanguageCode { get; set; }
        public string MeaningsLanguageCode { get; set; }
        public List<WordImageEntity> Images { get; set; }
        public List<WordSetWordEntity> Words { get; set; }
    }

    public class WordSetWordEntity : BaseEntity
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
        public List<string> Examples { get; set; }
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }
        public List<WordImageEntity> Images { get; set; }
    }
}
