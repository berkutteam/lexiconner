using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.WordSets.Validators
{
    public class WordSetsRequestDtoValidator : AbstractValidator<WordSetsRequestDto>
    {
        public WordSetsRequestDtoValidator()
        {
            RuleFor(x => x.LanguageCode).NotEmpty().MaximumLength(2);
        }
    }
}
