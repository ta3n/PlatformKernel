using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;

public record PriceTypeGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<PriceTypeResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
