using FluentValidation;

namespace Lexiconner.Application.ApplicationSettings.Validators
{
    public class CorsSettingsValidator : AbstractValidator<CorsSettings>
    {
        public CorsSettingsValidator()
        {
        }
    }
}
