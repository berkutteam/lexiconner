using FluentValidation;

namespace Lexiconner.Application.ApplicationSettings.Validators
{
    public class GoogleSettingsValidator : AbstractValidator<GoogleSettings>
    {
        public GoogleSettingsValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.WebApiServiceAccount).NotNull().ChildRules(v =>
            {
                v.RuleFor(x => x.Type).NotEmpty();
                v.RuleFor(x => x.ProjectId).NotEmpty();
                v.RuleFor(x => x.PrivateKeyId).NotEmpty();
                v.RuleFor(x => x.PrivateKey).NotEmpty();
                v.RuleFor(x => x.ClientEmail).NotEmpty();
                v.RuleFor(x => x.ClientId).NotEmpty();
                v.RuleFor(x => x.AuthUri).NotEmpty();
                v.RuleFor(x => x.TokenUri).NotEmpty();
                v.RuleFor(x => x.AuthProviderX509CertUrl).NotEmpty();
                v.RuleFor(x => x.ClientX509CertUrl).NotEmpty();
            });
        }
    }
}
