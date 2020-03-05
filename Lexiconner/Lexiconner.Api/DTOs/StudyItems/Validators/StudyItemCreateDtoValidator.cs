using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Api.DTOs.StudyItems.Validators
{
    public class StudyItemCreateDtoValidator : AbstractValidator<StudyItemCreateDto>
    {
        public StudyItemCreateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ExampleText).NotEmpty().MaximumLength(200);
            RuleFor(x => x.LanguageCode).NotEmpty();
        }
    }
}
