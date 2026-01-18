using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Sales;

public record SaleGetAllReservationsQuery(
    SaleGetAllRequest Request,
    IPageable Pageable
) : IQueryPagedBase<SaleDetailResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
