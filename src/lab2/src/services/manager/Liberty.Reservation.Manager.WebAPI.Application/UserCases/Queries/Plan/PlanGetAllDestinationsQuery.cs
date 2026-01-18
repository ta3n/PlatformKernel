using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

public record PlanGetAllDestinationsQuery(
    long Id,
    IPageable Pageable
) : IQueryPagedBase<SiteResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
