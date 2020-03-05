using FluentValidation.Results;
using Lexiconner.Application.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Application.Exceptions
{
    /// <summary>
    /// Indicates that validation was failed
    /// </summary>
    public class ValidationErrorException : ApplicationBaseException
    {
        private const string _title = "One or more validation errors occurred.";

        public ICollection<CustomValidationResult> ValidationFailures;

        public ValidationErrorException() : base(_title)
        {
            ValidationFailures = new List<CustomValidationResult>();
        }

        public ValidationErrorException(string message) : base(_title, message)
        {

        }

        public ValidationErrorException(ICollection<CustomValidationResult> validationFailures) : base(_title)
        {
            ValidationFailures = validationFailures;
        }
        protected ValidationErrorException(ICollection<CustomValidationResult> validationFailures, Exception innerException) : base(_title, "Validation error", innerException)
        {

        }
    }
}
