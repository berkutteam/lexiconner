using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words.Validators
{
    public class WordsRequestDtoValidator : AbstractValidator<WordsRequestDto>
    {
        public WordsRequestDtoValidator()
        {
            RuleFor(x => x.LanguageCode).NotEmpty().MaximumLength(2);
        }
    }
}
