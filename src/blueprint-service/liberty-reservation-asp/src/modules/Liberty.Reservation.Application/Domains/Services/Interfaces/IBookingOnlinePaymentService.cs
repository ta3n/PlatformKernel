namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Defines the contract for handling online payment operations related to reservations.
/// </summary>
public interface IBookingOnlinePaymentService
{
    /// <summary>
    /// Updates the amount for an online payment associated with a reservation.
    /// </summary>
    /// <param name="reservationId">The unique identifier of the reservation.</param>
    /// <param name="totalPrice">The new total price to be set for the reservation.</param>
    /// <param name="isRollback">
    /// A flag indicating whether the operation is a rollback.
    /// If <c>true</c>, the payment change will be reverted.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a tuple:
    /// <list type="bullet">
    /// <item>
    /// <description><c>transactionId</c>: The identifier of the payment transaction, or <c>null</c> if not applicable.</description>
    /// </item>
    /// <item>
    /// <description><c>existingOrderId</c>: The identifier of the existing order associated with the reservation.</description>
    /// </item>
    /// </list>
    /// </returns>
    Task<(string? transactionId, string existingOrderId)> OnlinePaymentChangeAmountAsync(
        long reservationId,
        decimal totalPrice,
        bool isRollback = false,
        CancellationToken cancellationToken = default
    );
}
