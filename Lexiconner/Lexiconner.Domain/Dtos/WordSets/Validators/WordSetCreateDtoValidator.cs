using FluentValidation;
using Lexiconner.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.WordSets.Validators
{
    public class WordSetCreateDtoValidator : AbstractValidator<WordSetCreateDto>
    {
        public WordSetCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.WordsLanguageCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.MeaningsLanguageCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.Words).NotNull().ListMustContainFewerThanOrEqual(1000);
            RuleForEach(x => x.Words).SetValidator(new WordSetCreateWordDtoValidator());
            RuleFor(x => x.Images).NotNull().ListMustContainFewerThanOrEqual(5);
        }
    }

    public class WordSetCreateWordDtoValidator : AbstractValidator<WordSetCreateWordDto>
    {
        public WordSetCreateWordDtoValidator()
        {
            RuleFor(x => x.Word).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Meaning).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Examples).NotNull().ListMustContainFewerThanOrEqual(5);
            RuleFor(x => x.Images).NotNull().ListMustContainFewerThanOrEqual(5);
        }
    }
}
