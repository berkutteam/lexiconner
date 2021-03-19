using Lexiconner.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.WordTrainings
{
    public class BuildWordTrainingResultDto
    {
        public TrainingType TrainingType { get; set; }
        public List<BuildWordTrainingResultForItemDto> ItemsResults { get; set; }
    }

    public class BuildWordTrainingResultForItemDto
    {
        public string WordId { get; set; }
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
