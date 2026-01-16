using System.Linq.Expressions;
using System.Reflection;

namespace PlatformKernel.Specification;

/// <summary>
/// Provides extension methods for working with specifications and expressions,
/// enabling composition, negation, and utility operations on specifications.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Combines two specifications into a new specification that represents the logical "AND" operation
    /// between the given specifications.
    /// </summary>
    /// <param name="left">The first specification to combine.</param>
    /// <param name="right">The second specification to combine.</param>
    /// <typeparam name="T">The type of the object to which the specifications apply.</typeparam>
    /// <returns>A new specification that represents the logical "AND" of the provided specifications.</returns>
    public static ISpecification<T> And<T>(
        this ISpecification<T> left,
        ISpecification<T> right
    )
    {
        return new AndSpec<T>(left, right);
    }

    /// <summary>
    /// Combines two specifications using a logical OR operation.
    /// </summary>
    /// <typeparam name="T">The type of object the specifications apply to.</typeparam>
    /// <param name="left">The first specification to combine.</param>
    /// <param name="right">The second specification to combine.</param>
    /// <returns>A new specification that represents the logical OR of the provided specifications.</returns>
    public static ISpecification<T> Or<T>(
        this ISpecification<T> left,
        ISpecification<T> right
    )
    {
        return new OrSpec<T>(left, right);
    }

    /// <summary>
    /// Negates the current specification, creating a new specification that represents
    /// the inverse of the original criteria.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the specification applies.</typeparam>
    /// <param name="inner">The original specification to be negated.</param>
    /// <returns>A new specification representing the negation of the original specification.</returns>
    public static ISpecification<T> Negate<T>(
        this ISpecification<T> inner
    )
    {
        return new NegatedSpec<T>(inner);
    }

    /// <summary>
    /// Applies sorting to the given specification object based on the specified parameters.
    /// </summary>
    /// <param name="gridSpec">
    /// The root specification to which the sorting logic will be applied.
    /// </param>
    /// <param name="sort">
    /// The sorting key indicating the property to sort by, optionally specifying a descending order
    /// via a "Desc" suffix.
    /// </param>
    /// <param name="orderByMethodName">
    /// The name of the method to be called for applying ascending order in the derived specification.
    /// </param>
    /// <param name="orderByDescendingMethodName">
    /// The name of the method to be called for applying descending order in the derived specification.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the specified property for sorting does not exist in the target type of the specification.
    /// </exception>
    public static void ApplySorting(
        this IRootSpecification gridSpec,
        string sort,
        string orderByMethodName,
        string orderByDescendingMethodName
    )
    {
        if (string.IsNullOrEmpty(sort))
        {
            return;
        }

        const string descendingSuffix = "Desc";

        var descending = sort.EndsWith(descendingSuffix, StringComparison.Ordinal);
        var propertyName = string.Concat(
            sort[..1].ToUpperInvariant(),
            sort.AsSpan(1, sort.Length - 1 - (descending ? descendingSuffix.Length : 0))
        );

        var specificationType = gridSpec.GetType().BaseType;
        var targetType = specificationType?.GenericTypeArguments[0];
        var property = targetType!.GetRuntimeProperty(propertyName)
            ?? throw new InvalidOperationException(
                $"Because the property {propertyName} does not exist it cannot be sorted."
            );

        var lambdaParamX = Expression.Parameter(targetType!, "x");

        var propertyReturningExpression = Expression.Lambda(
            Expression.Convert(
                Expression.Property(lambdaParamX, property),
                typeof(object)
            ),
            lambdaParamX
        );

        if (descending)
        {
            specificationType?.GetMethod(
                    orderByDescendingMethodName,
                    BindingFlags.Instance | BindingFlags.Public
                )
                ?.Invoke(
                    gridSpec,
                    [
                        propertyReturningExpression
                    ]
                );
        }
        else
        {
            specificationType?.GetMethod(
                    orderByMethodName,
                    BindingFlags.Instance | BindingFlags.Public
                )
                ?.Invoke(
                    gridSpec,
                    [
                        propertyReturningExpression
                    ]
                );
        }
    }

    /// <summary>
    /// Combines two expressions into one using a logical AND operation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expression.</typeparam>
    /// <param name="src">The first expression to combine.</param>
    /// <param name="dest">The second expression to combine.</param>
    /// <returns>A new expression representing a logical AND of the two expressions.</returns>
    public static Expression<Func<T, bool>> CombineExpressions<T>(
        this Expression<Func<T, bool>> src,
        Expression<Func<T, bool>> dest
    )
    {
        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(src, parameter),
            Expression.Invoke(dest, parameter)
        );
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
