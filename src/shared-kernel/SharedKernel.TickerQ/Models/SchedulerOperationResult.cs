namespace SharedKernel.TickerQ.Models;

/// <summary>
/// Represents the result of a scheduler operation.
/// </summary>
public sealed class SchedulerOperationResult
{
    /// <summary>
    /// Whether the operation succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The scheduled job identifier.
    /// </summary>
    public string? JobId { get; set; }

    /// <summary>
    /// Error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional metadata about the operation.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    public static SchedulerOperationResult Succeeded(
        string jobId,
        Dictionary<string, object>? metadata = null
    )
    {
        return new SchedulerOperationResult
        {
            Success = true,
            JobId = jobId,
            Metadata = metadata
        };
    }

    public static SchedulerOperationResult Failed(
        string errorMessage
    )
    {
        return new SchedulerOperationResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
