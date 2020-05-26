using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Lexiconner.Application.Exceptions;

namespace Lexiconner.Application.Validation
{
    /// <summary>
    /// Helper methods for DataAnnotations and FluentValidation
    /// </summary>
    public static class CustomValidationHelper
    {
        /// <summary>
        /// Validates and returns only custom results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static bool TryValidate<T>(T instance, out ICollection<CustomValidationResult> validationResults) where T: class
        {
            var isValid1 = DataAnnotationsValidationHelper.TryValidate(instance, out ICollection<CustomValidationResult> validationResults1);
            var isValid2 = FluentValidationHelper.TryValidate<T>(instance, out ICollection<CustomValidationResult> validationResults2);

            validationResults = validationResults1.Concat(validationResults2).ToList();

            return isValid1 && isValid2;
        }

        /// <summary>
        /// Validates instance and throws <see cref="ValidationErrorException" /> if validation failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void Validate<T>(T instance) where T : class
        {
            if (!TryValidate<T>(instance, out ICollection<CustomValidationResult> validationResults))
            {
                throw new ValidationErrorException(validationResults);
            }
        }

        /// <summary>
        /// Returns formatted validation error message
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static string GetValidationFormattedMessage(ICollection<CustomValidationResult> validationResults)
        {
            var validationErrorMessage = string.Join($"{Environment.NewLine}", validationResults.Select(x =>
            {
                return $"{x.PropertyName}: {x.ErrorMessage}";
            }));
            return $"One or more validation errors occurred:{Environment.NewLine}{validationErrorMessage}";
        }
    }
}
