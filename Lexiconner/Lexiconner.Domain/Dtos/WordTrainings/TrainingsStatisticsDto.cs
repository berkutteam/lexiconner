using Lexiconner.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.WordsTrainings
{
    public class TrainingsStatisticsDto
    {
        public TrainingsStatisticsDto()
        {
            TrainingStats = new List<TrainingStatisticsItemDto>();
        }

        public long TotalItemCount { get; set; }
        public long TrainedItemCount { get; set; }
        public long OnTrainingItemCount { get; set; }

        public List<TrainingStatisticsItemDto> TrainingStats { get; set; }

        public class TrainingStatisticsItemDto
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public TrainingType TrainingType { get; set; }
            public string TrainingTypeFormatted { get; set; }
            public long TrainedItemCount { get; set; }
            public long OnTrainingItemCount { get; set; }
        }
    }
}
