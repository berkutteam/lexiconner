using Lexiconner.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Dtos.StudyItemsTrainings
{
    public class WordMeaningTrainingResultDto
    {
        public TrainingType TrainingType { get; set; }
        public List<WordMeaningTrainingResultForItemDto> ItemsResults { get; set; }
    }

    public class WordMeaningTrainingResultForItemDto
    {
        public string ItemId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
