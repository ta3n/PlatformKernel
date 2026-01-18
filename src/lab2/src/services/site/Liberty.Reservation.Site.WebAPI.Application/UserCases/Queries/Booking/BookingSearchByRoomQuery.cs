using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.Application.Models.Responses;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public record BookingSearchByRoomQuery(
    BookingSearchPlanRequest Payload,
    IPageable Pageable
) : IQueryPagedBase<BookingSearchByRoomResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
