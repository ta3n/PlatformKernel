using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;

public record SaleGetAllReservationsQuery(
    long StartAppDateId,
    long EndAppDateId,
    IPageable Pageable
) : IQueryPagedBase<SaleDetailResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
