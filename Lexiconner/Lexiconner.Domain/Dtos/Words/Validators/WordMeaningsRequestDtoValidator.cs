using FluentValidation;
using Lexiconner.Domain.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words.Validators
{
    public class WordMeaningsRequestDtoValidator : AbstractValidator<WordMeaningsRequestDto>
    {
        public WordMeaningsRequestDtoValidator()
        {
            RuleFor(x => x.WordLanguageCode).NotEmpty().MaximumLength(10).Must(x => LanguageConfig.HasLanguageByCode(x)).WithMessage("Please specify valid language code.");
            RuleFor(x => x.MeaningLanguageCode).NotEmpty().MaximumLength(10).Must(x => LanguageConfig.HasLanguageByCode(x)).WithMessage("Please specify valid language code.");
            RuleFor(x => x.Word).NotEmpty().MaximumLength(200);
        }
    }
}
