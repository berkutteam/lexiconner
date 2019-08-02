using Lexiconner.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Enums
{
    public enum TrainingType
    {
        None = 0,

        [TrainingTypeInfo(trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */, correctAnswerProgressRate: 0.4, wrongAnswerProgressRate: -0.5)]
        FlashCards = 1,

        [TrainingTypeInfo(trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */, correctAnswerProgressRate: 0.25, wrongAnswerProgressRate: -0.75)]
        WordMeaning = 2,

        [TrainingTypeInfo(trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */, correctAnswerProgressRate: 0.35, wrongAnswerProgressRate: -0.75)]
        MeaningWord = 3,
    }
}
