using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetMonthsQuery() : IQuerySingleBase<IEnumerable<long>>;
