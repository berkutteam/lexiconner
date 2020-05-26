using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Lexiconner.Application.Validation
{
    /// <summary>
    /// Valdator that does nothing
    /// </summary>
    public class EmptyValidator<T> : AbstractValidator<T>
    {
        public EmptyValidator()
        {

        }
    }
}
