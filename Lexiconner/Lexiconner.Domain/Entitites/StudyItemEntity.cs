using Lexiconner.Domain.Entitites.Base;
using Lexiconner.Domain.Enums;
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
            Tags = new List<string>();
        }

        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ExampleText { get; set; }
        // public string ExampleImageUrl { get; set; }

        public List<string> Tags { get; set; }

        public StudyItemImageEntity Image { get; set; }
        public StudyItemTrainingInfoEntity TrainingInfo { get; set; }

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

        public double GetOverallProgress()
        {
            var progress = Math.Round(Trainings.Select(x => x.Progress).Sum() / Trainings.Count, 2);
            return progress;
        }



        public class StudyItemTrainingProgressItemEntity : BaseEntity
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public TrainingType TrainingType { get; set; }
            public DateTime LastTrainingdAt { get; set; }
            public DateTime NextTrainingdAt { get; set; }

            /// <summary>
            /// Percents [0, 1]
            /// </summary>
            public double Progress { get; set; }
        }
    }
}
