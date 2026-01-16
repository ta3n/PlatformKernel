using System.Linq.Expressions;

namespace PlatformKernel.Specification;

/// <summary>
/// Represents a specification pattern interface that defines a contract for specifying criteria
/// to query objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the object to which the specification applies.</typeparam>
public interface ISpecification<T> : IRootSpecification
{
    /// <summary>
    /// Represents the criteria expression used to determine whether an entity satisfies the specification.
    /// This property holds a lambda expression that defines the filtering condition.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Represents a collection of expressions specifying navigation properties to include
    /// in the query for eager loading.
    /// </summary>
    /// <remarks>
    /// The <c>Includes</c> property is a list of expressions where each expression defines
    /// a navigation property in the entity to be included in the query. This property is
    /// utilized to configure eager loading by including related entities in the query results.
    /// </remarks>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Represents a collection of string-based include paths for navigation properties
    /// to be included in a query. These strings specify the related entities that
    /// should be loaded alongside the main entity.
    /// </summary>
    /// <remarks>
    /// This property is commonly utilized in scenarios where string-based include
    /// paths are necessary, typically in combination with Entity Framework queries.
    /// It complements the <see cref="Includes"/> property which uses typed lambda expressions for navigation.
    /// </remarks>
    /// <value>
    /// A list of strings defining the navigation paths to include in query execution.
    /// Each string in the list represents a related entity path to be eagerly loaded.
    /// </value>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Represents an expression used to define the property and order to sort the results in ascending order.
    /// </summary>
    /// <remarks>
    /// This property is typically used in conjunction with queryable resources to specify an ascending order
    /// for the results based on the given key selector.
    /// </remarks>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets an expression used to apply a descending order sorting operation on a query.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the objects being queried.
    /// </typeparam>
    /// <remarks>
    /// This property is typically utilized in query expressions to specify the order
    /// in which records should be returned based on a specific key in descending order.
    /// </remarks>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Represents the grouping logic for a specification that is applied to a query.
    /// This property defines an expression used for grouping query results based on a specified key.
    /// </summary>
    /// <typeparam name="T">The type of the entity being queried.</typeparam>
    /// <remarks>
    /// When specified, the query will generate groupings according to the expression defined
    /// in the GroupBy property. The results of these groupings are flattened into a single sequence
    /// using the SelectMany operator, making it suitable for use in scenarios where grouped data
    /// needs to be enumerated.
    /// </remarks>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// Specifies the maximum number of records to retrieve in a query result.
    /// </summary>
    /// <remarks>
    /// When paging is enabled, this property determines the upper limit
    /// of items to fetch from the data source. This is typically used
    /// in combination with the <see cref="Skip"/> property to facilitate pagination.
    /// </remarks>
    int Take { get; }

    /// <summary>
    /// Gets the number of elements to bypass in the sequence.
    /// This property is primarily used in conjunction with pagination,
    /// allowing data queries to skip a specified number of records
    /// before beginning to retrieve the desired data set.
    /// </summary>
    int Skip { get; }

    /// Gets a value indicating whether paging is enabled for the query.
    /// When set to true, the query will utilize the Skip and Take properties to limit
    /// the number of items returned, providing pagination functionality. This property
    /// is typically used in conjunction with methods that configure paging, such as
    /// `ApplyPaging`.
    /// If false, the query will not apply paging and will return all matching results.
    bool IsPagingEnabled { get; }

    /// Determines whether the specified object satisfies the criteria defined in the specification.
    /// <param name="obj">The object to evaluate against the specification criteria.</param>
    /// <returns>True if the object satisfies the specification criteria; otherwise, false.</returns>
    bool IsSatisfiedBy(
        T obj
    );
}
