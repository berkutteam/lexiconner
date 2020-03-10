using FluentValidation;
using FluentValidation.Results;
using Lexiconner.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lexiconner.Application.Validation
{
    /// <summary>
    /// Helper methods for FluentValidation only
    /// </summary>
    public static class FluentValidationHelper
    {
        /// <summary>
        /// Validates and returns both custom results and original
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="validationResults"></param>
        /// <param name="originValidationResults"></param>
        /// <returns></returns>
        public static bool TryValidate<T>(T instance, out ICollection<CustomValidationResult> validationResults, out ICollection<ValidationFailure> originalValidationResults) where T : class
        {
            var isValid = TryValidate<T>(instance, out ICollection<ValidationFailure> internalResults);
            validationResults = ConvertToCustomResult(internalResults);
            originalValidationResults = internalResults;
            return isValid;
        }

        /// <summary>
        /// Validates and returns only custom results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static bool TryValidate<T>(T instance, out ICollection<CustomValidationResult> validationResults) where T : class
        {
            var isValid = TryValidate<T>(instance, out ICollection<ValidationFailure> internalResults);
            validationResults = ConvertToCustomResult(internalResults);
            return isValid;
        }

       

        /// <summary>
        /// Validates instance and throws <see cref="ValidationErrorException" /> if validation failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void Validate<T>(T instance) where T : class
        {
            if(!TryValidate<T>(instance, out ICollection<ValidationFailure> validationResults))
            {
                throw new ValidationErrorException(ConvertToCustomResult(validationResults));
            }
        }

        /// <summary>
        /// Returns formatted validation error message
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static string GetValidationFormattedMessage(ICollection<ValidationFailure> validationResults)
        {
            var validationErrorMessage = string.Join($"{Environment.NewLine}", validationResults.Select(x =>
            {
                return $"{x.PropertyName}: {x.ErrorMessage}";
            }));
            return $"One or more validation errors occurred:{Environment.NewLine}{validationErrorMessage}";
        }

        /// <summary>
        /// Converts FluentValidation result to custom result
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static ICollection<CustomValidationResult> ConvertToCustomResult(ICollection<ValidationFailure> validationResults)
        {
            return validationResults.Select(x => new CustomValidationResult
            {
                PropertyName = x.PropertyName,
                ErrorMessage = x.ErrorMessage
            }).ToList();
        }

        #region Private

        private static bool TryValidate<T>(T instance, out ICollection<ValidationFailure> results) where T : class
        {
            // We can get mismathced types when call method like
            // object obj = new SomeEntity();
            // TryValidate(obj);
            // in this case correct validator will not be found as
            // we will be searching for AbstractValidator<object>
            // typeof(T) == typeof(object)
            // instance.GetType() == typeof(SomeEntity)

            // check T is the same type as passed instance
            bool condition1 = !instance.GetType().Equals(typeof(T));

            // check T is the same or base type for passed instance
            bool condition2 = !typeof(T).IsAssignableFrom(instance.GetType());

            if (condition2) {
                throw new InternalErrorException("Type mismatch during validation. Recheck method signature.");
            }

            results = new List<ValidationFailure>();
            Type baseType = typeof(AbstractValidator<T>);

            List<TypeInfo> validatorTypes;
            try
            {
                validatorTypes = typeof(T).Assembly.DefinedTypes.Where(x => x.IsClass && x.BaseType != null && x.BaseType.Equals(baseType)).ToList();
            }
            catch(ReflectionTypeLoadException ex)
            {
                // can't load some assembly references
                var originColor = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"{nameof(FluentValidationHelper)} caught {nameof(ReflectionTypeLoadException)}: {ex.Message}");
                Console.BackgroundColor = originColor;

                return true;
            }

            if (validatorTypes.Any())
            {
                // validate
                var validators = validatorTypes.Select(type => Activator.CreateInstance(type) as AbstractValidator<T>);
                var validationResults = validators.Select(validator => validator.Validate(instance)).ToList();
                bool isValid = validationResults.All(x => x.IsValid);
                results = validationResults.SelectMany(x => x.Errors).ToList();
                return isValid;
            }

            return true;
        }

        #endregion
    }
}
