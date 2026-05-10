namespace SharedKernel.Hangfire.Models;

/// <summary>
/// Represents the result of a scheduler operation.
/// </summary>
public sealed record SchedulerOperationResult
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public required bool Succeeded { get; init; }

    /// <summary>
    /// Gets the stable scheduler job key.
    /// </summary>
    public required string JobKey { get; init; }

    /// <summary>
    /// Gets the optional Hangfire background job id.
    /// </summary>
    public string? HangfireJobId { get; init; }

    /// <summary>
    /// Gets the optional result message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Creates a successful scheduler operation result.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="hangfireJobId">The optional Hangfire background job id.</param>
    /// <param name="message">The optional result message.</param>
    /// <returns>The operation result.</returns>
    public static SchedulerOperationResult Success(
        string jobKey,
        string? hangfireJobId = null,
        string? message = null
    )
    {
        return new SchedulerOperationResult
        {
            Succeeded = true,
            JobKey = jobKey,
            HangfireJobId = hangfireJobId,
            Message = message
        };
    }

    /// <summary>
    /// Creates a failed scheduler operation result.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="message">The failure message.</param>
    /// <returns>The operation result.</returns>
    public static SchedulerOperationResult Failure(
        string jobKey,
        string message
    )
    {
        return new SchedulerOperationResult
        {
            Succeeded = false,
            JobKey = jobKey,
            Message = message
        };
    }
}
