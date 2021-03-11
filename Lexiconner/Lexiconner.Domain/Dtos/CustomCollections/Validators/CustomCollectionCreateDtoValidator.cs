using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.CustomCollections.Validators
{
    public class CustomCollectionCreateDtoValidator : AbstractValidator<CustomCollectionCreateDto>
    {
        public CustomCollectionCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}
