using Lexiconner.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.StudyItemsTrainings
{
    public class FlashCardsTrainingDto
    {
        public List<StudyItemEntity> Items { get; set; }
    }
}
