using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.UserDictionaries.Validators
{
    public class UserWordSetCreateDtoValidator : AbstractValidator<UserWordSetCreateDto>
    {
        public UserWordSetCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        }
    }
}
