using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;

public record SaleGetAllReservationsQuery(
    long StartAppDateId,
    long EndAppDateId,
    IPageable Pageable
) : IQueryPagedBase<SaleDetailResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
