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
    /// <summary>
    /// UserDictionary is a container for user words for specific language.
    /// </summary>
    public class UserDictionaryEntity : BaseEntity
    {
        public UserDictionaryEntity()
        {
            WordSets = new List<UserDictionaryWordSetEntity>();
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string WordsLanguageCode { get; set; }
        
        /// <summary>
        /// Saved or custom created word sets
        /// </summary>
        public List<UserDictionaryWordSetEntity> WordSets { get; set; }

        #region Helpers

        public UserDictionaryWordSetEntity AddDefaultWordSet()
        {
            var existing = this.WordSets.FirstOrDefault(x => x.IsDefault);
            if (existing == null)
            {
                existing = new UserDictionaryWordSetEntity()
                {
                    SourceWordSetId = null,
                    IsDefault = true,
                    Name = "Default",
                    WordsLanguageCode = this.WordsLanguageCode,
                    MeaningsLanguageCode = null,
                    Images = new List<GeneralImageEntity>(),
                };
                this.WordSets.Add(existing);
            }
            return existing;
        }

        public UserDictionaryWordSetEntity AddWordSet(WordSetEntity wordSet)
        {
            var existing = this.WordSets.FirstOrDefault(x => x.SourceWordSetId == wordSet.Id);
            if(existing == null)
            {
                existing = new UserDictionaryWordSetEntity()
                {
                    SourceWordSetId = wordSet.Id,
                    Name = wordSet.Name,
                    WordsLanguageCode = wordSet.WordsLanguageCode,
                    MeaningsLanguageCode = wordSet.MeaningsLanguageCode,
                    Images = JsonConvert.DeserializeObject<List<GeneralImageEntity>>(JsonConvert.SerializeObject(wordSet.Images)),
                };
                this.WordSets.Add(existing);
            }
            return existing;
        }

        public void DeleteWordSet(string wordSetId)
        {
            var existing = this.WordSets.FirstOrDefault(x => x.Id == wordSetId);
            if (existing != null)
            {
                this.WordSets.Remove(existing);
            }
        }

        #endregion
    }

    public class UserDictionaryWordSetEntity : BaseEntity
    {
        public UserDictionaryWordSetEntity()
        {
            Images = new List<GeneralImageEntity>();
        }

        /// <summary>
        /// Wordset which was the source of this user worset. Can be null if this a custom wordset.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string SourceWordSetId { get; set; }

        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public string WordsLanguageCode { get; set; }
        public string MeaningsLanguageCode { get; set; }
        public List<GeneralImageEntity> Images { get; set; }
    }
}
