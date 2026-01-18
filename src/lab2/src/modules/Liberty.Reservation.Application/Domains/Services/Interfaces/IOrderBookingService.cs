using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides methods to handle operations related to order bookings and their associations with reservations.
/// </summary>
public interface IOrderBookingService : IBaseServiceRelation<OrderReservation>
{
    /// <summary>
    /// Retrieves the order ID associated with the specified reservation ID.
    /// </summary>
    /// <param name="reservationId">The ID of the reservation whose associated order ID is to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The order ID related to the given reservation ID.</returns>
    /// <exception cref="ReservationInvalidException">Thrown if the reservation is not found or invalid.</exception>
    Task<long> GetOrderIdByReservationId(
        long reservationId,
        CancellationToken cancellationToken = default
    );
}
