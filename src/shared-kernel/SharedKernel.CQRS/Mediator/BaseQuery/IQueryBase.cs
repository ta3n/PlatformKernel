using global::Mediator;
using Microsoft.AspNetCore.Http;
using SharedKernel.Pagination;

namespace SharedKernel.CQRS.Mediator.BaseQuery;

/// <summary>
/// Defines the base contract for all query types in the CQRS pattern.
/// Queries are responsible for retrieving data without modifying the system state.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the data returned by the query.
/// It can represent a single object, a collection, or a combination of response and headers.
/// </typeparam>
public interface IQueryBase<TResponse>
    : IRequestBase<(IHeaderDictionary, TResponse)>, IQuery<(IHeaderDictionary, TResponse)>;

/// <summary>
/// Represents a base interface for queries that are intended to retrieve a single response of a specified type.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response that the query is expected to return.
/// </typeparam>
public interface IQuerySingleBase<TResponse> : IQueryBase<TResponse>;

/// <summary>
/// Represents a base interface for queries that retrieve a collection of responses.
/// </summary>
/// <typeparam name="TResponse">The type of the individual response elements in the collection.</typeparam>
public interface IQueryListBase<TResponse> : IQueryBase<IEnumerable<TResponse>>;

/// <summary>
/// Represents a contract for paged queries in the CQRS pattern.
/// Extends the base query contract to include support for pageable data
/// and a mechanism for retrieving partial sets of data in a paginated format.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the data returned by the query. Typically represents a collection of data
/// items obtained as part of a paged query result.
/// </typeparam>
public interface IQueryPagedBase<TResponse> : IQueryBase<IEnumerable<TResponse>>
{
    /// <summary>
    /// Represents pagination and sorting information for query results.
    /// </summary>
    /// <remarks>
    /// The Pageable property provides details about the pagination state, including the page number,
    /// page size, available sorting options, and whether pagination is enabled. It allows users to
    /// navigate through datasets and perform paged queries efficiently.
    /// </remarks>
    IPageable Pageable { get; set; }
}
