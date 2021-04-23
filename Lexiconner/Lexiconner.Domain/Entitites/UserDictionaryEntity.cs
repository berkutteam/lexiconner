using Lexiconner.Domain.Dtos.UserDictionaries;
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

        public bool AddDefaultWordSets()
        {
            const string allSet = "All";
            const string defaultSet = "Default";

            bool isChanged = false;

            //if(!this.WordSets.Any(x => x.Name == allSet))
            //{
            //    // logic wordset for All words
            //    this.WordSets.Add(new UserDictionaryWordSetEntity()
            //    {
            //        Id = null,
            //        SourceWordSetId = null,
            //        IsDefault = true,
            //        Name = allSet,
            //        WordsLanguageCode = this.WordsLanguageCode,
            //        MeaningsLanguageCode = null,
            //        Images = new List<GeneralImageEntity>(),
            //    });
            //    isChanged = true;
            //}
            if(!this.WordSets.Any(x => x.Name == defaultSet))
            {
                this.WordSets.Add(new UserDictionaryWordSetEntity()
                {
                    SourceWordSetId = null,
                    IsDefault = true,
                    Name = defaultSet,
                    WordsLanguageCode = this.WordsLanguageCode,
                    MeaningsLanguageCode = null,
                    Images = new List<GeneralImageEntity>(),
                });
                isChanged = true;
            }

            return isChanged;
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

        public void AddWordSet(UserWordSetCreateDto dto)
        {
            this.WordSets.Add(new UserDictionaryWordSetEntity()
            {
                Name = dto.Name,
                WordsLanguageCode = this.WordsLanguageCode,
                MeaningsLanguageCode = null,
            });
        }

        public void UpdateWordSet(string wordSetId, UserWordSetUpdateDto dto)
        {
            var existing = this.WordSets.FirstOrDefault(x => x.Id == wordSetId);
            if(existing != null)
            {
                existing.Name = dto.Name;
            }
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
