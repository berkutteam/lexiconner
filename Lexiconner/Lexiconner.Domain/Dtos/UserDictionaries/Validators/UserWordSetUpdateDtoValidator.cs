using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.UserDictionaries.Validators
{
    public class UserWordSetUpdateDtoValidator : AbstractValidator<UserWordSetUpdateDto>
    {
        public UserWordSetUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        }
    }
}
