using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Domain.Extensions
{
    // https://docs.fluentvalidation.net/en/latest/custom-validators.html
    public static class FluentValidationExtensions
    {
        //public static IRuleBuilderOptions<T, IEnumerable<TProperty>> ListMustContainFewerThan<T, TProperty>(this IRuleBuilder<T, IEnumerable<TProperty>> ruleBuilder, int length)
        //{
        //    return ruleBuilder
        //        .Must((rootObject, list, context) =>
        //        {
        //            context.MessageFormatter.AppendArgument("MaxElements", length);
        //            return list != null && list.Count() <= length;
        //        })
        //        .WithMessage("{PropertyName} must contain fewer than {MaxElements} items.");
        //}

        //public static IRuleBuilderOptions<T, List<TProperty>> ListMustContainFewerThan<T, TProperty>(this IRuleBuilder<T, List<TProperty>> ruleBuilder, int length)
        //{
        //    return ruleBuilder
        //        .Must((rootObject, list, context) =>
        //        {
        //            context.MessageFormatter.AppendArgument("MaxElements", length);
        //            return list != null && list.Count() <= length;
        //        })
        //        .WithMessage("{PropertyName} must contain fewer than {MaxElements} items.");
        //}

        public static IRuleBuilderOptionsConditions<T, IEnumerable<TProperty>> ListMustContainFewerThanOrEqual<T, TProperty>(this IRuleBuilder<T, IEnumerable<TProperty>> ruleBuilder, int length)
        {
            return ruleBuilder
                .Custom((list, context) =>
                {
                    if(list == null || list.Count() > length)
                    {
                        context.AddFailure(context.PropertyName, $"{context.PropertyName} must contain fewer than {length} items.");
                    }
                });
        }
    }
}
