using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.ApplicationSettings.Validators
{
    public class MicrosoftTranslatorSettingsValidator : AbstractValidator<MicrosoftTranslatorSettings>
    {
        public MicrosoftTranslatorSettingsValidator()
        {
            RuleFor(x => x.Endpoint).NotEmpty();
            RuleFor(x => x.SubscriptionKey).NotEmpty();
            RuleFor(x => x.Region).NotEmpty();
            RuleFor(x => x.ApiVersion).NotEmpty();
        }
    }
}
