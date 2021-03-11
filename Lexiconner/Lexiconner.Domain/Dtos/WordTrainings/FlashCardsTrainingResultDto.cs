using Lexiconner.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.WordsTrainings
{
    public class FlashCardsTrainingResultDto
    {
        public TrainingType TrainingType { get; set; }
        public List<FlashCardsTrainingResultForItemDto> ItemsResults { get; set; }
    }

    public class FlashCardsTrainingResultForItemDto
    {
        public string ItemId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
