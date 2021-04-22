﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Dtos.Words.Validators
{
    public class WordUpdateDtoValidator : AbstractValidator<WordUpdateDto>
    {
        public WordUpdateDtoValidator()
        {
            RuleFor(x => x.Word).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Meaning).NotEmpty().MaximumLength(200);
            RuleFor(x => x.WordLanguageCode).NotEmpty();
            //RuleFor(x => x.MeaningLanguageCode).NotEmpty(); // TODO
            RuleFor(x => x.UserWordSetId).NotEmpty();
        }
    }
}