using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationCheckNumberOfNightsQuery(
    long Id
) : IQuerySingleBase<bool>;
