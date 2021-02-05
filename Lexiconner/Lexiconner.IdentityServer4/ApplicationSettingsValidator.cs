using FluentValidation;
using Lexiconner.Application.ApplicationSettings;
using Lexiconner.Application.ApplicationSettings.Validators;

namespace Lexiconner.IdentityServer4
{
    public class ApplicationSettingsValidator : AbstractValidator<ApplicationSettings>
    {
        public ApplicationSettingsValidator()
        {
            RuleFor(x => x.Cors).SetValidator(new CorsSettingsValidator());
            RuleFor(x => x.MongoDb).SetValidator(new MongoDbSettingsValidator());
            RuleFor(x => x.Urls).SetValidator(new UrlsSettingsValidator());
            RuleFor(x => x.IdenitytServer4).SetValidator(new IdenitytServer4SettingsValidator());
        }
    }

    public class UrlsSettingsValidator : AbstractValidator<UrlsSettings>
    {
        public UrlsSettingsValidator()
        {
            RuleFor(x => x.SelfExternalUrl).NotEmpty();
            RuleFor(x => x.WebApiExternalUrl).NotEmpty();
            RuleFor(x => x.WebSpaExternalUrl).NotEmpty();
            RuleFor(x => x.WebSpaLocalUrl).NotEmpty();
            RuleFor(x => x.WebTestSpaExternalUrl).NotEmpty();
        }
    }

    public class IdenitytServer4SettingsValidator : AbstractValidator<IdenitytServer4Settings>
    {
        public IdenitytServer4SettingsValidator()
        {
            RuleFor(x => x.SigningCredential).NotNull().ChildRules(v =>
            {
                v.RuleFor(x => x.KeyStoreIssuer).NotEmpty();
                v.RuleFor(x => x.KeyFilePath).NotEmpty();
                v.RuleFor(x => x.KeyFilePathDeveloper).NotEmpty();
            });
        }
    }
}
