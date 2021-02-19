using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.Words.Validators
{
    public class UpdateWordImagesDtoValidator : AbstractValidator<UpdateWordImagesDto>
    {
        public UpdateWordImagesDtoValidator()
        {
            RuleForEach(x => x.Images).SetValidator(new WordImageUpdateDtoValidator());
        }
    }

    public class WordImageUpdateDtoValidator : AbstractValidator<WordImageUpdateDto>
    {
        public WordImageUpdateDtoValidator()
        {
            RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        }
    }
}
