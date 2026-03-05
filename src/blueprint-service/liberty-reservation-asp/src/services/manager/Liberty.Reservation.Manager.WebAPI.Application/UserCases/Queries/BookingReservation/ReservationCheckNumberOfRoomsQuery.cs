using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationCheckNumberOfRoomsQuery(
    long Id
) : IQuerySingleBase<bool>;
