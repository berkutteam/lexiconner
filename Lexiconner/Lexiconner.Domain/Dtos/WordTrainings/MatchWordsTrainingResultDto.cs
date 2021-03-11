using Lexiconner.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.Dtos.WordsTrainings
{
    public class MatchWordsTrainingResultDto
    {
        public TrainingType TrainingType { get; set; }
        public List<MatchWordsTrainingResultForItemDto> ItemsResults { get; set; }
    }

    public class MatchWordsTrainingResultForItemDto
    {
        public string ItemId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
