using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.StudyItems
{
    public class StudyItemDto
    {
        public StudyItemDto()
        {
            CustomCollectionIds = new List<string>();
            ExampleTexts = new List<string>();
            Tags = new List<string>();
            Images = new List<StudyItemImageDto>();
        }

        public string Id { get; set; }
        public List<string> CustomCollectionIds { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> ExampleTexts { get; set; }
        public bool IsFavourite { get; set; }
        public string LanguageCode { get; set; }
        public List<string> Tags { get; set; }

        public StudyItemImageDto Image { get; set; }
        public List<StudyItemImageDto> Images { get; set; }
        public StudyItemTrainingInfoDto TrainingInfo { get; set; }
    }

    public class StudyItemImageDto
    {
        public StudyItemImageDto()
        {
            RandomId = Guid.NewGuid().ToString();
        }

        public string RandomId { get; set; }
        public string Url { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailHeight { get; set; }
        public string ThumbnailWidth { get; set; }
        public string Base64Encoding { get; set; }
    }

    public class StudyItemTrainingInfoDto
    {
        public double TotalProgress { get; set; }
        public bool IsTrained { get; set; }
    }
}
