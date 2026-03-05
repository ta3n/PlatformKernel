using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public record BookingGetAllPriceCalendarsQuery(
    BookingSearchPriceCalendarRequest Payload
) : IQueryListBase<BookingCalendarPriceResponse>;
