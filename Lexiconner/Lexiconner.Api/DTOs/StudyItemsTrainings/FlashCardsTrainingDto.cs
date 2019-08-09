using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.StudyItemsTrainings
{
    public class FlashCardsTrainingDto
    {
        public TrainingType TrainingType => TrainingType.FlashCards;
        public List<StudyItemEntity> Items { get; set; }
    }
}
