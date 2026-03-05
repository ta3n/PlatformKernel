using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public record BookingSearchByPlanQuery(
    BookingSearchPlanRequest Payload,
    IPageable Pageable
) : IQueryPagedBase<BookingSearchByPlanResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
