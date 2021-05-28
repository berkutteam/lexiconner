using FluentValidation;

namespace Lexiconner.Domain.Dtos.Identity.Account.Validators
{
    public class BrowserExtensionRefreshTokensRequestDtoValidator : AbstractValidator<BrowserExtensionRefreshTokensRequestDto>
    {
        public BrowserExtensionRefreshTokensRequestDtoValidator()
        {
            RuleFor(x => x.IdentityToken).NotEmpty();
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ExtensionVersion).NotEmpty();
        }
    }
}
