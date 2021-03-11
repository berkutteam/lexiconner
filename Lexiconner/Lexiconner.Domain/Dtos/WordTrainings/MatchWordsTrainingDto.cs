using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Enums;
using Lexiconner.Domain.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Lexiconner.Api.Dtos.WordsTrainings
{
    public class MatchWordsTrainingDto
    {
        public MatchWordsTrainingDto()
        {
            Items = new List<MatchWordsTrainingItemDto>();
            PossibleOptions = new List<MatchWordsTrainingPossibleOptionDto>();
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TrainingType TrainingType => TrainingType.MatchWords;
        public string TrainingTypeFormatted => EnumHelper<TrainingType>.GetDisplayValue(TrainingType.MatchWords);
        public IEnumerable<MatchWordsTrainingItemDto> Items { get; set; }
        public IEnumerable<MatchWordsTrainingPossibleOptionDto> PossibleOptions { get; set; }
    }

    public class MatchWordsTrainingItemDto
    {
        public MatchWordsTrainingItemDto()
        {
        }

        public WordDto Word { get; set; }
        public string CorrectOptionId { get; set; }
    }

    public class MatchWordsTrainingPossibleOptionDto
    {
        public MatchWordsTrainingPossibleOptionDto()
        {
            RandomId = Guid.NewGuid().ToString();
        }

        public string RandomId { get; set; }
        public string Value { get; set; }
        public string CorrectForWordId { get; set; }
    }
}
