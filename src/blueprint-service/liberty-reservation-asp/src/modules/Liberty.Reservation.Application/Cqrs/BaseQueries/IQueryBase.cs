using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Pagination;

namespace Liberty.Reservation.Application.Cqrs.BaseQueries;

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
