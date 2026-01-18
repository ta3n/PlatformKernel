using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public record FacilityGetAllDestinationsQuery(
    IPageable Pageable
) : IQueryPagedBase<DestinationResponse>
{
    public long FacilityId { get; set; }
    public IPageable Pageable { get; set; } = Pageable;
}
