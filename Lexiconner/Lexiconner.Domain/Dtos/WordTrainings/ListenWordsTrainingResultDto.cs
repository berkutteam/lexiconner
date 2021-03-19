using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Domain.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Lexiconner.Domain.Dtos.WordTrainings
{
    public class ListenWordsTrainingResultDto
    {
        public ListenWordsTrainingResultDto()
        {
            ItemsResults = new List<ListenWordsTrainingResultForItemDto>();
        }

        public TrainingType TrainingType { get; set; }
        public List<ListenWordsTrainingResultForItemDto> ItemsResults { get; set; }
    }

    public class ListenWordsTrainingResultForItemDto
    {
        public string WordId { get; set; }
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
