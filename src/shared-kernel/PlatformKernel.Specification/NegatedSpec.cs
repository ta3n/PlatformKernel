using System.Linq.Expressions;

namespace PlatformKernel.Specification;

public class NegatedSpec<T>(
    ISpecification<T> inner
) : SpecificationBase<T>
{
    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var objParam = Expression.Parameter(typeof(T), "obj");

            var newExpr = Expression.Lambda<Func<T, bool>>(
                Expression.Not(
                    Expression.Invoke(inner.Criteria!, objParam)
                ),
                objParam
            );

            return newExpr;
        }
    }
}
