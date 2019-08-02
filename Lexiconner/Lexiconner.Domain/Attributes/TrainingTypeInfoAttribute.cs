using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TrainingTypeInfoAttribute : Attribute
    {
        public double TrainIntervalMs { get; set; }
        public double CorrectAnswerProgressRate { get; set; }
        public double WrongAnswerProgressRate { get; set; }

        public TimeSpan TrainIntervalTimespan => new TimeSpan(
            Convert.ToInt32(TrainIntervalMs / 1000 / 60 / 60),
            Convert.ToInt32(TrainIntervalMs / 1000 / 60),
            Convert.ToInt32(TrainIntervalMs / 1000)
        );

        public TrainingTypeInfoAttribute(
            double trainIntervalMs,
            double correctAnswerProgressRate,
            double wrongAnswerProgressRate
        )
        {
            TrainIntervalMs = trainIntervalMs;
            CorrectAnswerProgressRate = correctAnswerProgressRate;
            WrongAnswerProgressRate = wrongAnswerProgressRate;
        }
    }
}
