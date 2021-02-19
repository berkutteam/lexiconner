using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Domain.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Lexiconner.Api.DTOs.WordsTrainings
{
    public class FlashCardsTrainingDto
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TrainingType TrainingType => TrainingType.FlashCards;
        public string TrainingTypeFormatted => EnumHelper<TrainingType>.GetDisplayValue(TrainingType.FlashCards);
        public List<WordEntity> Items { get; set; }
    }
}
