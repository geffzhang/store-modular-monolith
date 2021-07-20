using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Core.Persistence.Specification
{
  public static class Extensions
    {
        public static ISpecification<T> And<T>(
            this ISpecification<T> left,
            ISpecification<T> right)
        {
            return new AndSpecification<T>(left, right);
        }

        public static ISpecification<T> Or<T>(
            this ISpecification<T> left,
            ISpecification<T> right)
        {
            return new OrSpecification<T>(left, right);
        }


        public static void ApplySorting<T>(this ISpecification<T> specification,
            string sort,
            string orderByDescendingMethodName,
            string groupByMethodName)
        {
            if (string.IsNullOrEmpty(sort)) return;

            const string descendingSuffix = "Desc";

            var descending = sort.EndsWith(descendingSuffix, StringComparison.Ordinal);
            var propertyName = sort.Substring(0, 1).ToUpperInvariant() +
                               sort.Substring(1, sort.Length - 1 - (descending ? descendingSuffix.Length : 0));

            var specificationType = specification.GetType().BaseType;
            var targetType = specificationType?.GenericTypeArguments[0];
            var property = targetType!.GetRuntimeProperty(propertyName) ??
                           throw new InvalidOperationException($"Because the property {propertyName} does not exist it cannot be sorted.");

            var lambdaParamX = Expression.Parameter(targetType, "x");

            var propertyReturningExpression = Expression.Lambda(
                Expression.Convert(
                    Expression.Property(lambdaParamX, property),
                    typeof(object)),
                lambdaParamX);

            if (descending)
            {
                specificationType?.GetMethod(
                        orderByDescendingMethodName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ?.Invoke(specification, new object[]{propertyReturningExpression});
            }
            else
            {
                specificationType?.GetMethod(
                        groupByMethodName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ?.Invoke(specification, new object[]{propertyReturningExpression});
            }
        }
    }
}