using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.DTOs.CustomCollections.Validators
{
    public class CustomCollectionUpdateDtoValidator : AbstractValidator<CustomCollectionUpdateDto>
    {
        public CustomCollectionUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
