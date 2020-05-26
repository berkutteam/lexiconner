using Lexiconner.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Lexiconner.Application.Validation
{
    /// <summary>
    /// Helper methods for DataAnnotations only
    /// </summary>
    public static class DataAnnotationsValidationHelper
    {
        /// <summary>
        /// Validates and returns both custom results and original
        /// </summary>
        /// <param name="object"></param>
        /// <param name="validationResults"></param>
        /// <param name="originalValidationResults"></param>
        /// <returns></returns>
        public static bool TryValidate(object @object, out ICollection<CustomValidationResult> validationResults, out ICollection<ValidationResult> originalValidationResults)
        {
            var isValid = TryValidate(@object, out ICollection<ValidationResult> internalResults);
            validationResults = ConvertToCustomResult(internalResults);
            originalValidationResults = internalResults;
            return isValid;
        }

        /// <summary>
        /// Validates and returns only custom results
        /// </summary>
        /// <param name="object"></param>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static bool TryValidate(object @object, out ICollection<CustomValidationResult> validationResults)
        {
            var isValid = TryValidate(@object, out ICollection<ValidationResult> internalResults);
            validationResults = ConvertToCustomResult(internalResults);
            return isValid;
        }

        /// <summary>
        /// Validates object and throws <see cref="ValidationErrorException" /> if validation failed
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static void Validate(object @object)
        {
            if (!TryValidate(@object, out ICollection<ValidationResult> validationResults))
            {
                throw new ValidationErrorException(ConvertToCustomResult(validationResults));
            }
        }

        /// <summary>
        /// Returns formatted validation error message
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static string GetValidationFormattedMessage(ICollection<ValidationResult> validationResults)
        {
            var validationErrorMessage = string.Join($"{Environment.NewLine}", validationResults.Select(x =>
            {
                return $"{string.Join(", ", x.MemberNames)}: {x.ErrorMessage}";
            }));
            return $"One or more validation errors occurred:{Environment.NewLine}{validationErrorMessage}";
        }

        /// <summary>
        /// Converts DataAnnotations result to custom result
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static ICollection<CustomValidationResult> ConvertToCustomResult(ICollection<ValidationResult> validationResults)
        {
            return validationResults.Select(x => new CustomValidationResult
            {
                PropertyName = string.Join(", ", x.MemberNames),
                ErrorMessage = x.ErrorMessage
            }).ToList();
        }

        #region Private

        private static bool TryValidate(object @object, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(instance: @object, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(@object, context, results, validateAllProperties: true);
        }

        #endregion
    }
}
