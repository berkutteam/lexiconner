using Lexiconner.Domain.Dtos.General;
using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words
{
    public class WordDto
    {
        public WordDto()
        {
            CustomCollectionIds = new List<string>();
            Examples = new List<string>();
            Tags = new List<string>();
            Images = new List<GeneralImageDto>();
        }

        public string Id { get; set; }
        public List<string> CustomCollectionIds { get; set; }
        public string UserDictionaryId { get; set; }
        public string UserWordSetId { get; set; }
        public string Word { get; set; }
        public string Meaning { get; set; }
        public List<string> Examples { get; set; }
        public bool IsFavourite { get; set; }
        public string WordLanguageCode { get; set; }
        public string MeaningLanguageCode { get; set; }
        public List<string> Tags { get; set; }

        public List<GeneralImageDto> Images { get; set; }
        public WordTrainingInfoDto TrainingInfo { get; set; }
    }

    public class WordTrainingInfoDto
    {
        public double TotalProgress { get; set; }
        public bool IsTrained { get; set; }
    }
}
