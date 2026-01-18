using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.UseCases.Queries.BookingReservation;

public record BookingSearchByPlanQuery(
    BookingSearchPlanRequest Payload,
    long FacilityId,
    long SiteId,
    IPageable Pageable
) : IQueryPagedBase<BookingSearchByPlanResponse>
{
    public bool IsLoadingPriceAppDate { get; init; } = true;
    public IPageable Pageable { get; set; } = Pageable;
}
