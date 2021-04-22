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


namespace Lexiconner.Domain.Entitites.General
{
    public class WordGeneralEntity : BaseEntity
    {
        public WordGeneralEntity()
        {
            Images = new List<GeneralImageEntity>();
        }

        public string Word { get; set; }
        public string Meaning { get; set; }
        public List<string> Examples { get; set; }

        /// <summary>
        /// ISO 639-1 two-letter code.
        /// https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// https://developers.google.com/admin-sdk/directory/v1/languages
        /// </summary>
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }

        public List<GeneralImageEntity> Images { get; set; }
    }
}
