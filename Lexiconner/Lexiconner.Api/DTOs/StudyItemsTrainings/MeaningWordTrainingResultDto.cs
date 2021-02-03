using Lexiconner.Domain.Enums;
using System.Collections.Generic;

namespace Lexiconner.Api.Dtos.StudyItemsTrainings
{
    public class MeaningWordTrainingResultDto
    {
        public TrainingType TrainingType { get; set; }
        public List<MeaningWordTrainingResultForItemDto> ItemsResults { get; set; }
    }

    public class MeaningWordTrainingResultForItemDto
    {
        public string ItemId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
