using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetDetailsQuery(
    long Id
) : IQuerySingleBase<ReservationDetailsResponse>;
