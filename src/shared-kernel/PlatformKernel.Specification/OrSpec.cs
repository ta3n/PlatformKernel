using System.Linq.Expressions;

namespace PlatformKernel.Specification;

public class OrSpec<T>(
    ISpecification<T> left,
    ISpecification<T> right
) : SpecificationBase<T>
{
    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var objParam = Expression.Parameter(typeof(T), "obj");

            var newExpr = Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(
                    Expression.Invoke(left.Criteria!, objParam),
                    Expression.Invoke(right.Criteria!, objParam)
                ),
                objParam
            );

            return newExpr;
        }
    }
}
