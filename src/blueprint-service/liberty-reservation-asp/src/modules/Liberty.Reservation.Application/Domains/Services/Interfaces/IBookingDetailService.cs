using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Defines the interface for retrieving detailed booking plan information and functionality
/// to support booking-related operations.
/// </summary>
public interface IBookingDetailService
{
    /// <summary>
    /// Determines if the system supports online payment functionality.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests and terminate the operation if requested.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> indicating whether the system can perform online payments.
    /// </returns>
    Task<bool> GetSystemCanOnlinePaymentAsync(
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Retrieves the state of a specific facility, including its payment capabilities
    /// and other configuration details.
    /// </summary>
    /// <param name="facilityId">
    /// The unique identifier of the facility.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests and terminate the operation if requested.
    /// </param>
    /// <returns>
    /// A <see cref="FacilityStateModel"/> object containing the state details of the specified facility.
    /// </returns>
    Task<FacilityStateModel> GetFacilitySateAsync(
        long facilityId,
        CancellationToken cancellationToken
    );
}
