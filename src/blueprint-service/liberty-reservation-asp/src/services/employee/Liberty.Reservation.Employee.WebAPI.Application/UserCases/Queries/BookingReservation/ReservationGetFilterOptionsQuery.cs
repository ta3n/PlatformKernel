using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetFilterOptionsQuery : IQuerySingleBase<ReservationFilterOptionsResponse>;
