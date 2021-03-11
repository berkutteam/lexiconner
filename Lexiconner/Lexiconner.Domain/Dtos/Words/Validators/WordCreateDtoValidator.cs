using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words.Validators
{
    public class WordCreateDtoValidator : AbstractValidator<WordCreateDto>
    {
        public WordCreateDtoValidator()
        {
            RuleFor(x => x.Word).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Meaning).NotEmpty().MaximumLength(200);
            RuleFor(x => x.WordLanguageCode).NotEmpty();
            RuleFor(x => x.MeaningLanguageCode).NotEmpty();
        }
    }
}
