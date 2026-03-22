using System.Linq.Expressions;
using SharedKernel.Specification;

namespace SharedKernel.Specification.Test;

public class UnitTest1
{
    [Fact]
    public void AndOrNegate_ComposeCriteriaCorrectly()
    {
        var adults = new PredicateSpecification(item => item.Age >= 18);
        var active = new PredicateSpecification(item => item.IsActive);

        Assert.True(adults.And(active).IsSatisfiedBy(new TestItem("Kernel", 20, true)));
        Assert.True(adults.Or(active).IsSatisfiedBy(new TestItem("Kernel", 15, true)));
        Assert.True(active.Negate().IsSatisfiedBy(new TestItem("Kernel", 20, false)));
    }

    [Fact]
    public void PredicateBuilder_BuildsExpectedComparisons()
    {
        var contains = PredicateBuilder.Build<TestItem>(nameof(TestItem.Name), "Contains", "ker").Compile();
        var greaterThan = PredicateBuilder.Build<TestItem>(nameof(TestItem.Age), ">=", "18").Compile();

        Assert.True(contains(new TestItem("kernel", 10, true)));
        Assert.True(greaterThan(new TestItem("demo", 18, true)));
        Assert.False(greaterThan(new TestItem("demo", 17, true)));
    }

    [Fact]
    public void CombineExpressions_UsesLogicalAnd()
    {
        Expression<Func<TestItem, bool>> left = item => item.Age >= 18;
        Expression<Func<TestItem, bool>> right = item => item.IsActive;

        var combined = left.CombineExpressions(right).Compile();

        Assert.True(combined(new TestItem("kernel", 22, true)));
        Assert.False(combined(new TestItem("kernel", 22, false)));
    }

    [Fact]
    public void ApplySorting_SetsAscendingAndDescendingOrderExpressions()
    {
        var specification = new SortableSpecification();

        specification.ApplyNameSort();
        Assert.NotNull(specification.OrderBy);
        Assert.Null(specification.OrderByDescending);

        specification.ApplyAgeDescending();
        Assert.NotNull(specification.OrderByDescending);
    }

    [Fact]
    public void ApplySorting_ThrowsWhenPropertyDoesNotExist()
    {
        var specification = new SortableSpecification();

        var exception = Assert.Throws<InvalidOperationException>(() => specification.ApplyMissingSort());

        Assert.Contains("does not exist", exception.Message, StringComparison.Ordinal);
    }

    private sealed record TestItem(string Name, int Age, bool IsActive);

    private sealed class PredicateSpecification(Expression<Func<TestItem, bool>> criteria) : SpecificationBase<TestItem>
    {
        public override Expression<Func<TestItem, bool>> Criteria { get; } = criteria;
    }

    private abstract class SortableSpecificationBase<T> : SpecificationBase<T>
    {
        public new void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => base.ApplyOrderBy(orderByExpression);

        public new void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) => base.ApplyOrderByDescending(orderByDescendingExpression);
    }

    private sealed class SortableSpecification : SortableSpecificationBase<TestItem>
    {
        public override Expression<Func<TestItem, bool>> Criteria => item => true;

        public void ApplyNameSort() => ApplySorting("name");

        public void ApplyAgeDescending() => ApplySorting("ageDesc");

        public void ApplyMissingSort() => ApplySorting("missing");
    }
}
