using FluentValidation;
using Lexiconner.Application.ApplicationSettings;
using Lexiconner.Application.ApplicationSettings.Validators;

namespace Lexiconner.Web
{
    public class ApplicationSettingsValidator : AbstractValidator<ApplicationSettings>
    {
        public ApplicationSettingsValidator()
        {
            RuleFor(x => x.Cors).SetValidator(new CorsSettingsValidator());
            RuleFor(x => x.ClientAuth).SetValidator(new ClientJwtBearerAuthSettingsValidator());
            RuleFor(x => x.Urls).SetValidator(new UrlsSettingsValidator());
        }
    }

    public class ClientJwtBearerAuthSettingsValidator : AbstractValidator<ClientJwtBearerAuthSettings>
    {
        public ClientJwtBearerAuthSettingsValidator()
        {
            RuleFor(x => x.Authority).NotEmpty();
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.RedirectUri).NotEmpty();
            RuleFor(x => x.ResponseType).NotEmpty();
            RuleFor(x => x.Scopes).NotEmpty();
            RuleFor(x => x.PostLogoutRedirectUri).NotEmpty();
        }
    }

    public class UrlsSettingsValidator : AbstractValidator<UrlsSettings>
    {
        public UrlsSettingsValidator()
        {
            RuleFor(x => x.SelfExternalUrl).NotEmpty();
            RuleFor(x => x.ApiExternalUrl).NotEmpty();
        }
    }
}
