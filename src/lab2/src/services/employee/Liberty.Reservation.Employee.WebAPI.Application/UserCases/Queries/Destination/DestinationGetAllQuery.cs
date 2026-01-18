using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;

public record DestinationGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<DestinationResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
