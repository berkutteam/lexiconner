using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.StudyItems.Validators
{
    public class UpdateWordImagesDtoValidator : AbstractValidator<UpdateWordImagesDto>
    {
        public UpdateWordImagesDtoValidator()
        {
            RuleForEach(x => x.Images).SetValidator(new StudyItemImageUpdateDtoValidator());
        }
    }

    public class StudyItemImageUpdateDtoValidator : AbstractValidator<StudyItemImageUpdateDto>
    {
        public StudyItemImageUpdateDtoValidator()
        {
            RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        }
    }
}
