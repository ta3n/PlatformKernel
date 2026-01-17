using System.Linq.Expressions;

namespace SharedKernel.Specification;

/// <summary>
/// Provides a utility class for building and combining predicate expressions dynamically.
/// </summary>
public static class PredicateBuilder
{
    /// Constructs a lambda expression representing a predicate based on the specified property name, comparison operator, and value.
    /// <param name="propertyName">The name of the property to be compared. Can include nested properties separated by dots.</param>
    /// <param name="comparison">The comparison operator to use, such as "==", "!=", ">", ">=", "Contains", "StartsWith", "EndsWith", or "In".</param>
    /// <param name="value">The value to compare the property against. For "In" comparison, values should be comma-separated.</param>
    /// <typeparam name="T">The type of the object to which the predicate applies.</typeparam>
    /// <returns>A lambda expression representing the constructed predicate.</returns>
    public static Expression<Func<T, bool>> Build<T>(
        string propertyName,
        string comparison,
        string value
    )
    {
        const string parameterName = "x";
        var parameter = Expression.Parameter(typeof(T), parameterName);
        var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
        var body = MakeComparison(left, comparison, value);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Combines two expressions with a logical "AND" operation, creating a new expression that evaluates to true only if both input expressions evaluate to true.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the lambda expression.</typeparam>
    /// <param name="a">The first expression to combine.</param>
    /// <param name="b">The second expression to combine.</param>
    /// <returns>
    /// A new expression that represents the logical "AND" of the two provided expressions.
    /// </returns>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> a,
        Expression<Func<T, bool>> b
    )
    {
        var p = a.Parameters[0];

        var visitor = new SubstExpressionVisitor { Subst = { [b.Parameters[0]] = p } };

        Expression body = Expression.And(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    /// <summary>
    /// Combines two expressions with a logical "OR" operation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expressions.</typeparam>
    /// <param name="a">The first expression to combine.</param>
    /// <param name="b">The second expression to combine.</param>
    /// <returns>An expression that represents the logical "OR" of the two input expressions.</returns>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> a,
        Expression<Func<T, bool>> b
    )
    {
        var p = a.Parameters[0];

        var visitor = new SubstExpressionVisitor { Subst = { [b.Parameters[0]] = p } };

        Expression body = Expression.Or(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    /// <summary>
    /// Creates an expression to compare a left-hand operand with a specified comparison operator and value.
    /// </summary>
    /// <param name="left">The left operand of the comparison, represented as an expression.</param>
    /// <param name="comparison">The comparison operator as a string (e.g., "==", "!=", ">", "Contains", ...).</param>
    /// <param name="value">The value to compare against, represented as a string.</param>
    /// <returns>An expression representing the comparison operation.</returns>
    /// <exception cref="NotSupportedException">Thrown when an invalid or unsupported comparison operator is provided.</exception>
    private static Expression MakeComparison(
        Expression left,
        string comparison,
        string value
    )
    {
        return comparison switch
        {
            "==" => MakeBinary(ExpressionType.Equal, left, value),
            "!=" => MakeBinary(ExpressionType.NotEqual, left, value),
            ">" => MakeBinary(ExpressionType.GreaterThan, left, value),
            ">=" => MakeBinary(ExpressionType.GreaterThanOrEqual, left, value),
            "<" => MakeBinary(ExpressionType.LessThan, left, value),
            "<=" => MakeBinary(ExpressionType.LessThanOrEqual, left, value),
            "Contains" or "StartsWith" or "EndsWith" => Expression.Call(
                MakeString(left),
                comparison,
                Type.EmptyTypes,
                Expression.Constant(value, typeof(string))
            ),
            "In" => MakeList(left, value.Split(',')),
            _ => throw new NotSupportedException($"Invalid comparison operator '{comparison}'.")
        };
    }

    /// <summary>
    /// Creates a MethodCallExpression to represent a collection 'Contains' operation
    /// for an enumerable of string values converted to a list of objects.
    /// </summary>
    /// <param name="left">The expression representing the property or field to evaluate.</param>
    /// <param name="codes">The enumerable containing string values to match against.</param>
    /// <returns>A <see cref="MethodCallExpression"/> representing the 'Contains' operation.</returns>
    private static MethodCallExpression MakeList(
        Expression left,
        IEnumerable<string> codes
    )
    {
        var objValues = codes.Cast<object>().ToList();
        var type = typeof(List<object>);
        var methodInfo = type.GetMethod(
            "Contains",
            [
                typeof(object)
            ]
        );
        var list = Expression.Constant(objValues);
        var body = Expression.Call(list, methodInfo!, left);
        return body;
    }

    /// Converts a given expression to a string type if it is not already of type string.
    /// If the source expression is already a string, it is returned as-is. Otherwise,
    /// the `ToString` method is invoked on the expression.
    /// <param name="source">The expression to be converted to a string.</param>
    /// <return>
    /// An `Expression` of type string representing the conversion of the source expression.
    /// If the source is already a string, it is returned unchanged.
    /// </return>
    private static Expression MakeString(
        Expression source
    )
    {
        return source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
    }

    /// <summary>
    /// Creates a binary expression given the type, left expression, and value.
    /// </summary>
    /// <param name="type">The type of binary operation to perform (e.g., equal, greater than, etc.).</param>
    /// <param name="left">The left-hand side of the binary expression.</param>
    /// <param name="value">The value to be used as the right-hand side of the binary expression.</param>
    /// <returns>A <see cref="BinaryExpression"/> representing the constructed binary operation.</returns>
    private static BinaryExpression MakeBinary(
        ExpressionType type,
        Expression left,
        string value
    )
    {
        object? typedValue = value;

        if (left.Type == typeof(string))
        {
            return Expression.MakeBinary(
                type,
                left,
                Expression.Constant(typedValue, left.Type)
            );
        }

        if (string.IsNullOrEmpty(value))
        {
            typedValue = null;
            if (Nullable.GetUnderlyingType(left.Type) == null)
            {
                left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
            }
        }
        else
        {
            var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
            if (valueType.IsEnum)
            {
                typedValue = Enum.Parse(valueType, value);
            }
            else
            {
                typedValue = valueType == typeof(Guid)
                    ? Guid.Parse(value)
                    : Convert.ChangeType(value, valueType);
            }
        }

        var right = Expression.Constant(typedValue, left.Type);
        return Expression.MakeBinary(type, left, right);
    }

    /// <summary>
    /// A specialized <see cref="ExpressionVisitor"/> used for substituting expressions within an expression tree.
    /// </summary>
    private sealed class SubstExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Defines a dictionary used for substituting expressions within an expression tree.
        /// The `Subst` dictionary maps expressions to be replaced with their corresponding substitutes
        /// during the expression tree transformation process. It is utilized in scenarios where
        /// parameter replacement is required, such as combining lambda expressions with logical
        /// operators like And and Or.
        /// </summary>
        public readonly Dictionary<Expression, Expression> Subst = [];

        /// <summary>
        /// Visits a <see cref="ParameterExpression"/> during the traversal of an expression tree.
        /// Replaces the parameter nodes with the corresponding substituted expressions if found in the substitution map.
        /// </summary>
        /// <param name="node">The parameter expression to evaluate and potentially replace.</param>
        /// <returns>
        /// The original parameter expression if no substitution is found in the substitution map,
        /// or the substituted expression if a replacement exists.
        /// </returns>
        protected override Expression VisitParameter(
            ParameterExpression node
        )
        {
            return Subst.GetValueOrDefault(node, node);
        }
    }
}
