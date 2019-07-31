using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Enums
{
    public enum TrainingType
    {
        None = 0,

        // TrainInterval = 1day, CorrectAnswerProgressRate = 0.2, WrongAnswerProgressRate = -0.5
        FlashCards = 1,
        WordMeaning = 2,
        MeaningWord = 3,
    }
}
