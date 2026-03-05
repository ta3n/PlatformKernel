using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationCheckNumberOfNightsQuery(
    long Id
) : IQuerySingleBase<bool>;
