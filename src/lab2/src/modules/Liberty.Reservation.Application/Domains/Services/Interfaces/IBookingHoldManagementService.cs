using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides services for managing and interacting with room booking holds.
/// </summary>
public interface IBookingHoldManagementService
{
    /// <summary>
    /// Attempts to hold a room for the specified booking details.
    /// Validates if the requested number of rooms can be held within the availability constraints
    /// and applies a hold if the request is feasible.
    /// </summary>
    /// <param name="model">The model containing booking details such as facility, site, plan, room, check-in dates, and number of rooms.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the room hold was successfully applied; otherwise, false.</returns>
    Task<bool> TryHoldRoomAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Releases the hold on a room based on the provided booking hold check model.
    /// </summary>
    /// <param name="model">
    /// The model containing information about the booking hold, such as facility, site, plan, room,
    /// and the number of rooms and nights to release the hold on.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to signal that the operation should be canceled.
    /// This parameter is optional.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous release operation.
    /// </returns>
    Task ReleaseHoldAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the availability of rooms for each night within a specified booking period.
    /// </summary>
    /// <param name="model">
    /// The <see cref="BookingHoldCheckModel"/> containing details such as facility, site, plan, room, check-in date,
    /// number of nights, and number of rooms required for availability calculations.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A dictionary where the key is the date represented as a long ID, and the value is the number of rooms available for that date.
    /// </returns>
    Task<Dictionary<long, int>> GetAvailabilityForUserAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks the room availability for guest confirmation scenarios, such as when a guest is about to confirm a booking.
    /// </summary>
    /// <param name="model">
    /// The <see cref="BookingHoldCheckModel"/> containing booking details including facility, site, plan, room, check-in date, number of nights, and number of rooms.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the operation to complete, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a dictionary mapping each date (as a long ID) to the number of rooms available for that date.
    /// </returns>
    /// <example>
    /// <code>
    /// var availability = await bookingHoldManagementService.CheckAvailabilityForGuestConfirmAsync(model, cancellationToken);
    /// </code>
    /// </example>
    Task<Dictionary<long, int>> CheckAvailabilityForGuestConfirmAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    );
}
