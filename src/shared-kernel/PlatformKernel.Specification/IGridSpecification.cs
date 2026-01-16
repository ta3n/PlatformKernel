using System.Linq.Expressions;

namespace PlatformKernel.Specification;

/// <summary>
/// Represents a specification pattern interface designed to define queries for grid-based data retrieval,
/// including filtering, sorting, grouping, and pagination behavior. This interface allows combining query
/// criteria, associated includes, and pagination properties for flexible and reusable query definitions.
/// </summary>
/// <typeparam name="T">The type of entities targeted by this specification.</typeparam>
public interface IGridSpecification<T> : IRootSpecification
{
    /// <summary>
    /// Gets the collection of expressions used to define filtering criteria
    /// for querying data. Each expression represents a condition that must
    /// be satisfied by the entities in the query result. The collection can
    /// include multiple expressions to apply complex filtering logic.
    /// </summary>
    List<Expression<Func<T, bool>>> Criteria { get; }

    /// <summary>
    /// A collection of expressions specifying related entities to include in the query result.
    /// </summary>
    /// <remarks>
    /// This property consists of a list of lambda expressions, where each expression defines
    /// a navigation property to be included in the query. These expressions allow for the
    /// inclusion of related data during query execution, supporting eager loading scenarios.
    /// </remarks>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// A collection of navigation property paths represented as strings.
    /// The paths are used to specify related entities to include in the query execution.
    /// </summary>
    /// <remarks>
    /// This property is particularly useful for including navigation properties
    /// that cannot be specified using strongly-typed expressions.
    /// </remarks>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the property that represents the expression used to define the ascending order
    /// for the result set. When specified, the result set will be ordered by the provided
    /// expression.
    /// </summary>
    /// <value>
    /// An expression that defines the property by which the result set should be ordered
    /// in ascending order. It can be null if no ordering is specified.
    /// </value>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets a lambda expression used to specify the descending order of sorting for the query.
    /// </summary>
    /// <remarks>
    /// This property is typically used in scenarios where a query needs to be sorted by a specific field
    /// in descending order. If set, it overrides the default ordering behavior for the query.
    /// </remarks>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the expression used to group elements.
    /// This property allows defining a grouping logic for query operations,
    /// enabling the classification of query results into groups based on a specified key.
    /// The resulting groups can then be processed or queried further as needed.
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// Specifies the maximum number of records or entities to be retrieved
    /// when the query is executed. It is used in scenarios requiring
    /// pagination or limiting the result set size.
    /// When combined with <see cref="Skip"/>, it enables efficient
    /// pagination by defining the number of items to retrieve after
    /// skipping a specified number of items.
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Gets the number of records to skip before starting to return results. This property is primarily used
    /// to enable paging in queries by specifying the starting point in the result set.
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// Gets or sets a value indicating whether paging is enabled for the query.
    /// When set to true, the query will apply paging logic using the <see cref="Skip"/>
    /// and <see cref="Take"/> properties. If false, all available data is queried
    /// without applying paging.
    /// </summary>
    bool IsPagingEnabled { get; set; }
}
