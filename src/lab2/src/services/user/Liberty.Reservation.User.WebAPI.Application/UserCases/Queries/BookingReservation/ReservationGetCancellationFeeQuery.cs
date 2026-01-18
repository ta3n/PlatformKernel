using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationGetCancellationFeeQuery(
    long Id
) : IQuerySingleBase<BookingCancellationFeeResponse>;
