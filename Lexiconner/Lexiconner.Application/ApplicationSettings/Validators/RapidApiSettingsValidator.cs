using FluentValidation;

namespace Lexiconner.Application.ApplicationSettings.Validators
{
    public class RapidApiSettingsValidator : AbstractValidator<RapidApiSettings>
    {
        public RapidApiSettingsValidator()
        {
            RuleFor(x => x.ContextualWebSearch).NotNull().ChildRules(v =>
            {
                v.RuleFor(x => x.ApplicationKey).NotEmpty();
            });
            RuleFor(x => x.TwinwordWordDictionary).NotNull().ChildRules(v =>
            {
                v.RuleFor(x => x.ApiUrl).NotEmpty();
                v.RuleFor(x => x.RapidApiHost).NotEmpty();
                v.RuleFor(x => x.RapidApiKey).NotEmpty();
            });
        }
    }
}
