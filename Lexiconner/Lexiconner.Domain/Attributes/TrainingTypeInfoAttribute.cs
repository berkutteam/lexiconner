using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TrainingTypeInfoAttribute : Attribute
    {
        /// <summary>
        /// Train interval when word progress < 1
        /// </summary>
        public double TrainIntervalMs { get; set; }

        /// <summary>
        /// Train interval when word progress == 1 (repeat trained word)
        /// </summary>
        public double TrainIntervalForRepeatMs { get; set; }

        public double CorrectAnswerProgressRate { get; set; }
        public double WrongAnswerProgressRate { get; set; }

        public TimeSpan TrainIntervalTimespan => new TimeSpan(
            Convert.ToInt32(TrainIntervalMs / 1000 / 60 / 60),
            Convert.ToInt32(TrainIntervalMs / 1000 / 60),
            Convert.ToInt32(TrainIntervalMs / 1000)
        );

        public TimeSpan TrainIntervalForRepeatTimespan => new TimeSpan(
          Convert.ToInt32(TrainIntervalForRepeatMs / 1000 / 60 / 60),
          Convert.ToInt32(TrainIntervalForRepeatMs / 1000 / 60),
          Convert.ToInt32(TrainIntervalForRepeatMs / 1000)
      );

        public TrainingTypeInfoAttribute(
            double trainIntervalMs,
            double trainIntervalForRepeatMs,
            double correctAnswerProgressRate,
            double wrongAnswerProgressRate
        )
        {
            TrainIntervalMs = trainIntervalMs;
            TrainIntervalForRepeatMs = trainIntervalForRepeatMs;
            CorrectAnswerProgressRate = correctAnswerProgressRate;
            WrongAnswerProgressRate = wrongAnswerProgressRate;
        }
    }
}
