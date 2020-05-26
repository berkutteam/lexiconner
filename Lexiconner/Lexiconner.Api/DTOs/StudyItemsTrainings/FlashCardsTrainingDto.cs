using Lexiconner.Application.Helpers;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.StudyItemsTrainings
{
    public class FlashCardsTrainingDto
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TrainingType TrainingType => TrainingType.FlashCards;
        public string TrainingTypeFormatted => EnumHelper<TrainingType>.GetDisplayValue(TrainingType.FlashCards);
        public List<StudyItemEntity> Items { get; set; }
    }
}
