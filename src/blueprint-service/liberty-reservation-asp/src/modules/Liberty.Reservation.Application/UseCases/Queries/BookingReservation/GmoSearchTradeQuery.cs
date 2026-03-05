using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.UseCases.Queries.BookingReservation;

public record GmoSearchTradeQuery(
    long ReservationId
) : IQuerySingleBase<SearchTradeResponse>;
