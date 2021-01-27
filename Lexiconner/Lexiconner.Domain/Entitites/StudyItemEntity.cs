using Lexiconner.Domain.Dtos.StudyItems;
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
    public class StudyItemEntity : BaseEntity
    {
        public StudyItemEntity()
        {
            CustomCollectionIds = new List<string>();
            ExampleTexts = new List<string>();
            Tags = new List<string>();
            TrainingInfo = new StudyItemTrainingInfoEntity();
        }

        public string UserId { get; set; }
        public List<string> CustomCollectionIds { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> ExampleTexts { get; set; }
        public bool IsFavourite { get; set; }

        /// <summary>
        /// ISO 639-1 two-letter code.
        /// https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// https://developers.google.com/admin-sdk/directory/v1/languages
        /// </summary>
        public string LanguageCode { get; set; }

        public List<string> Tags { get; set; }

        public StudyItemImageEntity Image { get; set; }
        public StudyItemTrainingInfoEntity TrainingInfo { get; set; }


        #region Helper methods

        public void UpdateSelf(StudyItemUpdateDto dto)
        {
            this.CustomCollectionIds = dto.CustomCollectionIds;
            if (this.Title != dto.Title)
            {
                this.Image = null;
            }
            this.Title = dto.Title;
            this.Description = dto.Description;
            this.ExampleTexts = dto.ExampleTexts;
            this.IsFavourite = dto.IsFavourite;
            this.LanguageCode = dto.LanguageCode;
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

    public class StudyItemImageEntity : BaseEntity
    {
        public string Url { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailHeight { get; set; }
        public string ThumbnailWidth { get; set; }
        public string Base64Encoding { get; set; }
    }

    public class StudyItemTrainingInfoEntity : BaseEntity
    {
        public StudyItemTrainingInfoEntity()
        {
            Trainings = new List<StudyItemTrainingProgressItemEntity>();
        }

        public List<StudyItemTrainingProgressItemEntity> Trainings { get; set; }

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

        public class StudyItemTrainingProgressItemEntity : BaseEntity
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
