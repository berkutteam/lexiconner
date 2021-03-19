using Lexiconner.Domain.Dtos.Words;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using Lexiconner.Domain.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
namespace Lexiconner.Domain.Dtos.WordTrainings
{
    public class BuildWordTrainingDto
    {
        public BuildWordTrainingDto()
        {
            Items = new List<BuildWordTrainingItemDto>();
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TrainingType TrainingType => TrainingType.BuildWord;
        public string TrainingTypeFormatted => EnumHelper<TrainingType>.GetDisplayValue(TrainingType.BuildWord);
        public IEnumerable<BuildWordTrainingItemDto> Items { get; set; }
    }

    public class BuildWordTrainingItemDto
    {
        public WordDto Word { get; set; }

        /// <summary>
        /// Letters and symbols, including spaces, dashed, etc
        /// </summary>
        public IEnumerable<char> WordParts { get; set; }

        /// <summary>
        /// Letters and symbols, including spaces, dashed, etc
        /// </summary>
        public IEnumerable<char> CorrectWordParts { get; set; }

        /// <summary>
        /// The result word that is supposed to be entered
        /// </summary>
        public string CorrectAnswer { get; set; }
    }
}
