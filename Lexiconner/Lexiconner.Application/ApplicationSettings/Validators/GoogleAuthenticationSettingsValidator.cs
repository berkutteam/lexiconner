using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.ApplicationSettings.Validators
{
    public class GoogleAuthenticationSettingsValidator : AbstractValidator<GoogleAuthenticationSettings>
    {
        public GoogleAuthenticationSettingsValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).NotEmpty();
        }
    }
}
