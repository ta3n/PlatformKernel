using System.Linq.Expressions;

namespace Liberty.Pagination;

/// <summary>
/// Represents a collection of sorting expressions that can be applied to a queryable source.
/// This class is used to construct and manage multiple sorting rules for an entity type.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity being sorted.
/// </typeparam>
/// <typeparam name="TKey">
/// The type of the key used in the sorting expressions.
/// </typeparam>
public class SortExpressions<TEntity, TKey>
{
    /// <summary>
    /// Represents a private list of sorting expressions used to define the order in which
    /// items of type <typeparamref name="TEntity"/> are sorted based on the specified key of type
    /// <typeparamref name="TKey"/>.
    /// </summary>
    /// <remarks>
    /// Each sorting expression in the list includes a lambda expression defining the sorting key
    /// and a boolean indicating whether the sorting should be descending.
    /// This field supports operations such as adding, retrieving, and applying sorting criteria
    /// to a collection of entities.
    /// </remarks>
    private readonly List<SortExpression<TEntity, TKey>> _expressionList = [];

    /// <summary>
    /// Adds a sorting expression to the current list of sort expressions.
    /// </summary>
    /// <param name="expression">
    /// The sorting expression that determines the property of the entity to sort by.
    /// </param>
    /// <param name="isDescending">
    /// Indicates whether the sorting should be in descending order.
    /// Default value is <c>false</c>, meaning the sorting will be in ascending order.
    /// </param>
    public void Add(
        Expression<Func<TEntity, TKey>> expression,
        bool isDescending = false
    )
    {
        _expressionList.Add(
            new SortExpression<TEntity, TKey>
            {
                Expression = expression,
                IsDescending = isDescending
            }
        );
    }

    /// <summary>
    /// Applies the specified sorting expressions to the given queryable data source.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities in the queryable data source.</typeparam>
    /// <typeparam name="TKey">The type of the key used in the sorting expressions.</typeparam>
    /// <param name="query">
    /// The queryable data source to which the sorting expressions will be applied.
    /// </param>
    /// <param name="sorts">
    /// A collection of sorting expressions that define the sorting parameters to apply to the queryable data source.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> representing the sorted queryable data source.
    /// If no valid sorting expressions are provided, the original queryable data source is returned.
    /// </returns>
    public static IQueryable<TEntity> ApplySorts(
        IQueryable<TEntity> query,
        SortExpressions<TEntity, TKey> sorts
    )
    {
        var isFirstSort = true;
        var validSortings = sorts.GetAll();
        IOrderedQueryable<TEntity>? orderedQuery = null;

        foreach (var sort in validSortings)
        {
            orderedQuery = isFirstSort
                ? ApplySorting(query, sort)
                : ApplySorting(orderedQuery, sort);
            isFirstSort = false;
        }

        return orderedQuery ?? query;
    }

    /// <summary>
    /// Retrieves all sort expressions stored in the collection.
    /// </summary>
    /// <returns>
    /// A dynamic collection containing the list of sort expressions with their associated sorting order.
    /// </returns>
    private dynamic GetAll()
    {
        return _expressionList;
    }

    /// <summary>
    /// Determines whether the sort expressions are valid.
    /// </summary>
    /// <returns>
    /// Returns true if the sort expressions list is not empty; otherwise, false.
    /// </returns>
    internal bool IsValid()
    {
        return _expressionList.Count != 0;
    }

    /// <summary>
    /// Applies sorting to the provided query based on the given sort expression.
    /// </summary>
    /// <param name="query">The source query to which sorting is applied.</param>
    /// <param name="sortExpression">The sort expression containing the property to sort by and the sort direction.</param>
    /// <returns>An ordered queryable based on the specified sort expression.</returns>
    private static IOrderedQueryable<TEntity> ApplySorting(
        IQueryable<TEntity> query,
        SortExpression<TEntity, TKey> sortExpression
    )
    {
        return sortExpression.IsDescending
            ? query.OrderByDescending(sortExpression.Expression!)
            : query.OrderBy(sortExpression.Expression!);
    }

    /// <summary>
    /// Applies additional sorting to an already ordered query using the specified sort expression.
    /// </summary>
    /// <param name="query">
    /// The ordered query to which additional sorting will be applied.
    /// </param>
    /// <param name="sortExpression">
    /// The sort expression containing the sorting logic and the order (ascending or descending).
    /// </param>
    /// <returns>
    /// The query with the additional sorting applied.
    /// </returns>
    private static IQueryable<TEntity> ApplySorting(
        IOrderedQueryable<TEntity> query,
        SortExpression<TEntity, TKey> sortExpression
    )
    {
        return sortExpression.IsDescending
            ? query.ThenByDescending(sortExpression.Expression!)
            : query.ThenBy(sortExpression.Expression!);
    }
}
