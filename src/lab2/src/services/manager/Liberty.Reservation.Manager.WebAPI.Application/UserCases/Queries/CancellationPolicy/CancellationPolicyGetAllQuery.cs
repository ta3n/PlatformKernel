using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;

public record CancellationPolicyGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<CancellationPolicyResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
