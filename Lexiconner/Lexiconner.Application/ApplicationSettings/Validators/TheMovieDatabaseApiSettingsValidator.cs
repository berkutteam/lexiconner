using FluentValidation;

namespace Lexiconner.Application.ApplicationSettings.Validators
{
    public class TheMovieDatabaseApiSettingsValidator : AbstractValidator<TheMovieDatabaseApiSettings>
    {
        public TheMovieDatabaseApiSettingsValidator()
        {
            RuleFor(x => x.ApiKeyV3Auth).NotEmpty();
            RuleFor(x => x.ApiKeyV4Auth).NotEmpty();
        }
    }
}
