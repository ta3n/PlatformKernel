using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public record BookingGetCalendarPricesQuery(
    long PlanId,
    long RoomGroupId,
    BookingSearchPlanRequest Payload
) : IQuerySingleBase<PriceCalendarOfRoomResponse>;
