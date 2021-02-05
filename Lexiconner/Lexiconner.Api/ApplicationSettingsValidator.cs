using FluentValidation;
using Lexiconner.Application.ApplicationSettings.Validators;

namespace Lexiconner.Api
{
    public class ApplicationSettingsValidator : AbstractValidator<ApplicationSettings>
    {
        public ApplicationSettingsValidator()
        {
            RuleFor(x => x.Cors).SetValidator(new CorsSettingsValidator());
            RuleFor(x => x.MongoDb).SetValidator(new MongoDbSettingsValidator());
            RuleFor(x => x.JwtBearerAuth).SetValidator(new JwtBearerAuthSettingsValidator());
            RuleFor(x => x.Urls).SetValidator(new UrlsSettingsValidator());
            RuleFor(x => x.Google).SetValidator(new GoogleSettingsValidator());
            RuleFor(x => x.RapidApi).SetValidator(new RapidApiSettingsValidator());
            RuleFor(x => x.TheMovieDatabase).SetValidator(new TheMovieDatabaseApiSettingsValidator());
        }
    }

    public class MongoDbSettingsValidator : AbstractValidator<MongoDbSettings>
    {
        public MongoDbSettingsValidator()
        {
            RuleFor(x => x.ConnectionString).NotEmpty();
            RuleFor(x => x.DatabaseMain).NotEmpty();
            RuleFor(x => x.DatabaseIdentity).NotEmpty();
            RuleFor(x => x.DatabaseSharedCache).NotEmpty();
        }
    }

    public class JwtBearerAuthSettingsValidator : AbstractValidator<JwtBearerAuthSettings>
    {
        public JwtBearerAuthSettingsValidator()
        {
            RuleFor(x => x.Authority).NotEmpty();
            RuleFor(x => x.Audience).NotEmpty();
            RuleFor(x => x.WebApiScope).NotEmpty();
        }
    }

    public class UrlsSettingsValidator : AbstractValidator<UrlsSettings>
    {
        public UrlsSettingsValidator()
        {
            RuleFor(x => x.SelfExternalUrl).NotEmpty();
        }
    }
}
