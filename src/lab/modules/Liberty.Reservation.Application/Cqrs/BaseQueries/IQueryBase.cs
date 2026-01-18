using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Pagination;

namespace Liberty.Reservation.Application.Cqrs.BaseQueries;

public interface IQueryPagedBase<TResponse> : IQueryBase<IEnumerable<TResponse>>
{
    IPageable Pageable { get; set; }
}
