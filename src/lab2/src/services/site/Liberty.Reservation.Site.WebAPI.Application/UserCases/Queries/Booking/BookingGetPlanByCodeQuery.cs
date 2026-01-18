using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public record BookingGetPlanByCodeQuery(
    string PlanCode,
    string RoomGroupCode
)
: IQuerySingleBase<BookingDetailsResponse>;
