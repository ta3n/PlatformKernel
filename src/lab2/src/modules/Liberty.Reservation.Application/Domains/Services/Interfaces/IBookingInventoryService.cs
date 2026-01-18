using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides methods for retrieving room inventory information for booking operations.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for fetching available room inventory
/// based on booking criteria such as facility, room, and plan details.
/// </remarks>
/// <example>
/// <code>
/// var inventory = await bookingInventoryService.GetRoomInventoryAsync(model, cancellationToken);
/// </code>
/// </example>
public interface IBookingInventoryService
{
    /// <summary>
    /// Retrieves the inventory of rooms based on the provided booking hold check model.
    /// </summary>
    /// <param name="model">The model containing details such as facility, room, and plan information for which room inventory needs to be checked.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of room inventory models.</returns>
    Task<IEnumerable<RoomInventoryModel>> GetRoomInventoryAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    );
}
