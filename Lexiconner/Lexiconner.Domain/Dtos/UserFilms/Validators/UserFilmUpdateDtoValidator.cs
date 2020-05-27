using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Dtos.UserFilms.Validators
{
    public class UserFilmUpdateDtoValidator : AbstractValidator<UserFilmUpdateDto>
    {
        public UserFilmUpdateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.MyRating).GreaterThanOrEqualTo(0);
            RuleFor(x => x.LanguageCode).NotEmpty();
        }
    }
}
