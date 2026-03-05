using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

public record PlanGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<PlanResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
