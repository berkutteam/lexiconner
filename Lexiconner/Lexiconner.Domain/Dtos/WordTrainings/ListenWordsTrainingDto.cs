using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Domain.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Lexiconner.Domain.Dtos.WordTrainings
{
    public class ListenWordsTrainingDto
    {
        public ListenWordsTrainingDto()
        {
            Items = new List<ListenWordsTrainingItemDto>();
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TrainingType TrainingType => TrainingType.ListenWords;
        public string TrainingTypeFormatted => EnumHelper<TrainingType>.GetDisplayValue(TrainingType.ListenWords);
        public IEnumerable<ListenWordsTrainingItemDto> Items { get; set; }
    }

    public class ListenWordsTrainingItemDto
    {
        public WordDto Word { get; set; }

        public string WordPronunciationAudioUrl { get; set; }

        /// <summary>
        /// The result word that is supposed to be entered
        /// </summary>
        public string CorrectAnswer { get; set; }
    }
}
