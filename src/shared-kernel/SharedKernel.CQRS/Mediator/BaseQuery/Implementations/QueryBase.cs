using SharedKernel.Pagination;

namespace SharedKernel.CQRS.Mediator.BaseQuery.Implementations;

/// <summary>
/// Represents the base class for all query types in the CQRS pattern.
/// Queries derived from this base class support retrieving data without modifying the system state.
/// </summary>
/// <typeparam name="TResponse">
/// The type of response returned by the query. This can support various formats such as single objects,
/// collections, or combinations of data and metadata.
/// </typeparam>
public abstract record QueryBase<TResponse> : RequestBase<TResponse>, IQueryBase<TResponse>;

/// <summary>
/// Represents a base record type for queries designed to retrieve a single response of a specified type.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response that the query is expected to return.
/// </typeparam>
public abstract record QuerySingleBase<TResponse> : QueryBase<TResponse>, IQuerySingleBase<TResponse>;

/// <summary>
/// Represents an abstract base class for queries that retrieve a collection of responses in CQRS pattern.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the individual response elements in the collection.
/// </typeparam>
public abstract record QueryListBase<TResponse> : QueryBase<IEnumerable<TResponse>>, IQueryListBase<TResponse>;

/// <summary>
/// Represents a base class for paginated queries in the CQRS pattern.
/// This abstraction facilitates the creation and handling of paginated
/// query results by implementing a pageable structure for data retrieval.
/// Extends the basic query functionality to include pagination capabilities.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the data returned by the query. Usually represents a paginated
/// collection of results.
/// </typeparam>
public abstract record QueryPagedBase<TResponse>(
    IPageable Pageable
) : QueryBase<IEnumerable<TResponse>>, IQueryPagedBase<TResponse>
{
    /// <summary>
    /// Represents pagination information used for executing paged queries.
    /// This property provides the required data for managing paginated responses,
    /// such as page size, current page, sorting information, and navigation capabilities.
    /// </summary>
    public IPageable Pageable { get; set; } = Pageable;
}
