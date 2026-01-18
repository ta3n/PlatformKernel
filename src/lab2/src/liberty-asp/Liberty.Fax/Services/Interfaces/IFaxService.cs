using Liberty.Fax.Models.Responses;

namespace Liberty.Fax.Services.Interfaces;

/// <summary>
/// Defines the contract for sending faxes.
/// </summary>
public interface IFaxService
{
    /// Sends a fax with the specified details asynchronously.
    /// <param name="processKey">A unique identifier for the process or transaction associated with the fax.</param>
    /// <param name="faxNumber">The recipient's fax number to which the fax will be sent.</param>
    /// <param name="subject">The subject or title of the fax being sent.</param>
    /// <param name="body">The message content or body of the fax.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a FaxResponse object with information about the fax result, or null if sending fails.</returns>
    Task<FaxResponse?> SendFaxAsync(
        long processKey,
        string faxNumber,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    );
}
