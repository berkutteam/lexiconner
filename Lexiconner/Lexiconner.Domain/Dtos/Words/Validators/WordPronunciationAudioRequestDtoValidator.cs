using FluentValidation;
using Lexiconner.Domain.Config;
using System;
using System.Collections.Generic;
using System.Text;


namespace Lexiconner.Domain.Dtos.Words.Validators
{
    public class WordPronunciationAudioRequestDtoValidator : AbstractValidator<WordPronunciationAudioRequestDto>
    {
        public WordPronunciationAudioRequestDtoValidator()
        {
            RuleFor(x => x.LanguageCode).NotEmpty().MaximumLength(10).Must(x => LanguageConfig.HasLanguageByCode(x)).WithMessage("Please specify valid language code.");
            RuleFor(x => x.Word).NotEmpty().MaximumLength(200);
        }
    }
}
