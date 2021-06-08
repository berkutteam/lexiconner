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
    public class WordEntity : WordGeneralEntity
    {
        public WordEntity() : base()
        {
            CustomCollectionIds = new List<string>();
            Examples = new List<string>();
            Images = new List<GeneralImageEntity>();
            Tags = new List<string>();
            TrainingInfo = new WordTrainingInfoEntity();
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> CustomCollectionIds { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserDictionaryId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserWordSetId { get; set; }

        /// <summary>
        /// Wordset word which was the source of this user word. Can be null if this word is't added from word set.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string SourceWordSetWordId { get; set; }

        public bool IsFavourite { get; set; }
        
        /// <summary>
        /// Indicates word added via browser extension
        /// </summary>
        public bool IsFromBrowserExtension { get; set; }

        public List<string> Tags { get; set; }
        public WordTrainingInfoEntity TrainingInfo { get; set; }


        #region Helper methods

        public void UpdateSelf(WordUpdateDto dto)
        {
            this.CustomCollectionIds = dto.CustomCollectionIds;
            if (this.Word != dto.Word)
            {
                this.Images = new List<GeneralImageEntity>();
            }
            this.Word = dto.Word;
            this.Meaning = dto.Meaning;
            this.Examples = dto.Examples;
            this.IsFavourite = dto.IsFavourite;
            this.WordLanguageCode = dto.WordLanguageCode;
            this.MeaningLanguageCode = dto.MeaningLanguageCode;
            this.Tags = dto.Tags;
        }

        public bool RemoveCollection(string collectionId)
        {
            bool isUpdated = false;
            var existing = this.CustomCollectionIds.FirstOrDefault(x => x == collectionId);
            if(existing != null)
            {
                this.CustomCollectionIds.Remove(existing);
                isUpdated = true;
            }
            return isUpdated;
        }

        public bool MarkAsTrained()
        {
            bool isUpdated = false;
            if (!this.TrainingInfo.IsTrained)
            {
                this.TrainingInfo.IsTrained = true;
                this.TrainingInfo.TrainedAt = DateTimeOffset.UtcNow;
                isUpdated = true;
            }
            return isUpdated;
        }

        public bool MarkAsNotTrained()
        {
            bool isUpdated = false;
            if (this.TrainingInfo.IsTrained)
            {
                this.TrainingInfo.IsTrained = false;
                this.TrainingInfo.NotTrainedAt = DateTimeOffset.UtcNow;
                isUpdated = true;
            }
            return isUpdated;
        }

        public bool RecalculateTotalTrainingProgress()
        {
            bool isUpdated = false;

            double currentProgress;
            if(this.TrainingInfo.IsTrained)
            {
                currentProgress = 1.0;
            } 
            else
            {
                currentProgress = Math.Round(
                   this.TrainingInfo.Trainings.Select(x => x.Progress).Sum() / this.TrainingInfo.Trainings.Count
               );
            }

            this.TrainingInfo.TotalProgress = currentProgress;
            isUpdated = true;

            return isUpdated;
        }

        public bool ResetTotalTrainingProgress()
        {
            this.TrainingInfo.TotalProgress = 0;
            return true;
        }


        #endregion
    }

    public class WordTrainingInfoEntity : BaseEntity
    {
        public WordTrainingInfoEntity()
        {
            Trainings = new List<WordTrainingProgressItemEntity>();
        }

        public List<WordTrainingProgressItemEntity> Trainings { get; set; }

        /// <summary>
        /// Total training progress. Percents [0, 1]
        /// </summary>
        public double TotalProgress { get; set; }

        /// <summary>
        /// Indicates that item is completely trained.
        /// </summary>
        public bool IsTrained { get; set; }

        /// <summary>
        /// When was learned or marked as trained
        /// </summary>
        public DateTimeOffset? TrainedAt { get; set; }

        /// <summary>
        /// When was marked as not trained
        /// </summary>
        public DateTimeOffset? NotTrainedAt { get; set; }

        public double GetOverallProgress()
        {
            var progress = Math.Round(Trainings.Select(x => x.Progress).Sum() / Trainings.Count, 2);
            return progress;
        }

        public class WordTrainingProgressItemEntity : BaseEntity
        {
            [JsonConverter(typeof(StringEnumConverter))]
            [BsonRepresentation(BsonType.String)]
            public TrainingType TrainingType { get; set; }
            public DateTimeOffset LastTrainingdAt { get; set; }
            public DateTimeOffset NextTrainingdAt { get; set; }

            /// <summary>
            /// Percents [0, 1]
            /// </summary>
            public double Progress { get; set; }
        }
    }
}
