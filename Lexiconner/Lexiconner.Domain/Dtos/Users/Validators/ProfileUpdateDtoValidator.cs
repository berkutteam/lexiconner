using FluentValidation;

namespace Lexiconner.Domain.Dtos.Users.Validators
{
    public class ProfileUpdateDtoValidator : AbstractValidator<ProfileUpdateDto>
    {
        public ProfileUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NativeLanguageCode).NotEmpty().MaximumLength(3);
        }
    }
}
