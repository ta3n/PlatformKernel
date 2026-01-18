using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Models.Responses;

namespace Liberty.GmoPaymentGateway.Services;

/// <summary>
/// Defines a service for interacting with the GMO payment gateway.
/// Provides methods for payment transactions such as searching trades,
/// cancelling orders, initiating transactions, executing transactions,
/// and retrieving payment URLs.
/// </summary>
public interface IGmoPaymentGatewayService
{
    /// <summary>
    /// Searches for trade details based on the provided request.
    /// </summary>
    /// <param name="request">The search trade request containing the necessary information such as the order ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the response of the trade search operation, including trade details.</returns>
    Task<OnLinePaymentSearchTradeResponse> SearchTradeAsync(
        SearchTradeRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cancels an online payment asynchronously using the specified cancellation request details.
    /// </summary>
    /// <param name="request">
    /// An instance of <see cref="CancelOrderRequest"/> containing the details required to cancel an order.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an instance of
    /// <see cref="OnLinePaymentCancelResponse"/> with the results of the cancellation request.
    /// </returns>
    Task<OnLinePaymentCancelResponse> CancelAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default
    );

    /// Initiates a payment entry transaction with the provided request details.
    /// <param name="request">
    /// The payment entry transaction request containing details such as order ID, card number,
    /// CVV, amount, tax, and currency.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, with a default value of CancellationToken.None.
    /// </param>
    /// <return>
    /// A task that represents the asynchronous operation. The task result contains an
    /// <see cref="EntryTranResponse"/> object that holds the response details of the initiated transaction.
    /// </return>
    Task<EntryTranResponse> EntryTranAsync(
        PaymentEntryTranRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a transaction using the specified payment details.
    /// </summary>
    /// <param name="request">
    /// The details of the payment transaction, including the order ID, access credentials,
    /// card information, and expiration date.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an
    /// <see cref="ExecTranResponse"/> which provides details about the transaction result.
    /// </returns>
    Task<ExecTranResponse> ExecTranAsync(
        PaymentExecTranRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a payment URL using the specified request parameters.
    /// </summary>
    /// <param name="request">
    /// An instance of <see cref="PaymentGetUrlRequest"/> containing the necessary details to generate the payment URL,
    /// such as OrderId, Amount, Tax, and optional LanguageCode.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, with a default value of <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the generated payment URL as a string.
    /// </returns>
    Task<string> GetPaymentUrlAsync(
        PaymentGetUrlRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Processes a change order request by updating the existing transaction details
    /// such as amount, tax, payment method, or installments in the GMO payment gateway.
    /// </summary>
    /// <param name="request">The change order request containing new transaction details including order ID, access details, and updated payment information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the response of the change order operation, including details of the updated transaction.</returns>
    Task<OnlinePaymentChangeResponse> ChangeOrderAsync(
        ChangeOrderRequest request,
        CancellationToken cancellationToken = default
    );
}
