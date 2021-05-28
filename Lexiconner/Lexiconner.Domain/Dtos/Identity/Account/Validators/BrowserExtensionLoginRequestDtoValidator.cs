using FluentValidation;

namespace Lexiconner.Domain.Dtos.Identity.Account.Validators
{
    public class BrowserExtensionLoginRequestDtoValidator : AbstractValidator<BrowserExtensionLoginRequestDto>
    {
        public BrowserExtensionLoginRequestDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ExtensionVersion).NotEmpty();
        }
    }
}
