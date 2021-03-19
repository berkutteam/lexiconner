using Lexiconner.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lexiconner.Domain.Enums
{
    public enum TrainingType
    {
        None = 0,

        [Display(Name = "Flash cards")]
        [TrainingTypeInfo(
            trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */, 
            trainIntervalForRepeatMs: 5 * 24 * 60 * 60 * 1000 /* 5 days */, 
            correctAnswerProgressRate: 0.4, 
            wrongAnswerProgressRate: -0.5
        )]
        FlashCards = 1,

        [Display(Name = "Word-Meaning")]
        [TrainingTypeInfo(
            trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */, 
            trainIntervalForRepeatMs: 5 * 24 * 60 * 60 * 1000 /* 5 days */, 
            correctAnswerProgressRate: 0.25, 
            wrongAnswerProgressRate: -0.75
        )]
        WordMeaning = 2,

        [Display(Name = "Meaning-Word")]
        [TrainingTypeInfo(
            trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */, 
            trainIntervalForRepeatMs: 5 * 24 * 60 * 60 * 1000 /* 5 days */, 
            correctAnswerProgressRate: 0.35, 
            wrongAnswerProgressRate: -0.75
        )]
        MeaningWord = 3,

        [Display(Name = "Match words")]
        [TrainingTypeInfo(
            trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */,
            trainIntervalForRepeatMs: 5 * 24 * 60 * 60 * 1000 /* 5 days */,
            correctAnswerProgressRate: 0.35,
            wrongAnswerProgressRate: -0.75
        )]
        MatchWords = 4,

        [Display(Name = "Build word")]
        [TrainingTypeInfo(
           trainIntervalMs: 24 * 60 * 60 * 1000 /* 24 hours */,
           trainIntervalForRepeatMs: 5 * 24 * 60 * 60 * 1000 /* 5 days */,
           correctAnswerProgressRate: 0.35,
           wrongAnswerProgressRate: -0.75
        )]
        BuildWord = 5,
    }

    public static class TrainingTypeHelper
    {
        public static TrainingTypeInfoAttribute GetAttribute(TrainingType trainingType)
        {
            ValidateAllEnumMembersHasAttribute();

            Type enumType = typeof(TrainingType);
            TrainingTypeInfoAttribute infoAttribute = null;
            foreach (var name in Enum.GetNames(enumType))
            {
                FieldInfo fieldInfo = enumType.GetField(name);
                TrainingType enumValue = (TrainingType)fieldInfo.GetValue(enumType);

                MemberInfo memberInfo = enumType.GetMember(name).First();

                if(enumValue == trainingType)
                {
                    infoAttribute = memberInfo.GetCustomAttribute<TrainingTypeInfoAttribute>();
                }
            }

            if(infoAttribute == null)
            {
                throw new Exception($"Can't find {typeof(TrainingTypeInfoAttribute).Name} for {trainingType}");
            }
            return infoAttribute;
        }

        public static void ValidateAllEnumMembersHasAttribute()
        {
            Type enumType = typeof(TrainingType);
            foreach (var name in Enum.GetNames(enumType))
            {
                MemberInfo memberInfo = enumType.GetMember(name).First();
                TrainingTypeInfoAttribute trainingTypeInfoAttribute = memberInfo.GetCustomAttribute<TrainingTypeInfoAttribute>();

                if(trainingTypeInfoAttribute == null && name != nameof(TrainingType.None))
                {
                    throw new Exception($"Each member of {typeof(TrainingType).Name} must have {typeof(TrainingTypeInfoAttribute).Name}!");
                }
            }
        }
    }
}
