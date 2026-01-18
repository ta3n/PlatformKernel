using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public record FacilityGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<FacilityResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
