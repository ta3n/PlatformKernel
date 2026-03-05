using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationCheckNumberOfRoomsQuery(
    long Id
) : IQuerySingleBase<bool>;
