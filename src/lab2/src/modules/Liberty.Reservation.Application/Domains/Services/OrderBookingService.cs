using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class OrderBookingService(
    ILogger<OrderBookingService> logger,
    IBookingOrderReservationRepository bookingOrderReservationRepository
) : BaseServiceRelation<OrderReservation>(logger, bookingOrderReservationRepository), IOrderBookingService
{
    public async Task<long> GetOrderIdByReservationId(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var reservation =
            await bookingOrderReservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.ReservationId == reservationId)
                .Select(x => new { x.OrderId })
                .SingleOrDefaultAsync(cancellationToken)
            ?? throw new ReservationInvalidException("Order Not Found");

        return reservation.OrderId;
    }
}
