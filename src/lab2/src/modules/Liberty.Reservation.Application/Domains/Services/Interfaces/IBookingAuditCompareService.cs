using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Defines the contract for services that compare booking audit logs and retrieve booking tree data.
/// </summary>
public interface IBookingAuditCompareService
{
    /// <summary>
    /// Retrieves all booking tree data for a given booking ID.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to retrieve the tree for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of
    /// <see cref="BookingTreeModel"/> representing the booking tree.
    /// </returns>
    Task<List<BookingTreeModel>> GetAllBookingTreeAsync(
        long bookingId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Compares two booking history data models and retrieves all audit logs detailing the changes.
    /// </summary>
    /// <param name="bookingNew">The new booking history data model.</param>
    /// <param name="bookingOld">The old booking history data model.</param>
    /// <returns>
    /// A list of <see cref="ChangeOfBookingAuditLogResponse"/> representing the changes between the two bookings.
    /// </returns>
    List<ChangeOfBookingAuditLogResponse> GetAllBookingAuditLogs(
        BookingHistoryDataModel bookingNew,
        BookingHistoryDataModel bookingOld
    );
}
