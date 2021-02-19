using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Enums;
using Lexiconner.Domain.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Lexiconner.Api.Dtos.WordsTrainings
{
    public class MeaningWordTrainingDto
    {
        public MeaningWordTrainingDto()
        {
            Items = new List<MeaningWordTrainingItemDto>();
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TrainingType TrainingType => TrainingType.MeaningWord;
        public string TrainingTypeFormatted => EnumHelper<TrainingType>.GetDisplayValue(TrainingType.MeaningWord);
        public IEnumerable<MeaningWordTrainingItemDto> Items { get; set; }
    }

    public class MeaningWordTrainingItemDto
    {
        public MeaningWordTrainingItemDto()
        {
            PossibleOptions = new List<MeaningWordTrainingOptionDto>();
        }

        public WordDto Word { get; set; }
        public IEnumerable<MeaningWordTrainingOptionDto> PossibleOptions { get; set; }
    }

    public class MeaningWordTrainingOptionDto
    {
        public MeaningWordTrainingOptionDto()
        {
            RandomId = Guid.NewGuid().ToString();
        }

        public string RandomId { get; set; }
        public string Value { get; set; }
        public bool IsCorrect { get; set; }
    }
}
