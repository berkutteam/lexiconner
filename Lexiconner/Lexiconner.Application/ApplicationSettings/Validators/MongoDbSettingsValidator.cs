using FluentValidation;

namespace Lexiconner.Application.ApplicationSettings.Validators
{
    public class MongoDbSettingsValidator : AbstractValidator<MongoDbSettings>
    {
        public MongoDbSettingsValidator()
        {
            RuleFor(x => x.ConnectionString).NotEmpty();
            RuleFor(x => x.Database).NotEmpty();
        }
    }
}
