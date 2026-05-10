namespace SharedKernel.Hangfire.Exceptions;

/// <summary>
/// Represents a transient scheduler job failure that should be retried by Hangfire.
/// </summary>
public sealed class RetryableSchedulerJobException : SchedulerJobException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RetryableSchedulerJobException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    public RetryableSchedulerJobException(
        string message
    ) : base(
        message,
        true
    )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryableSchedulerJobException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="innerException">The inner exception.</param>
    public RetryableSchedulerJobException(
        string message,
        Exception innerException
    ) : base(
        message,
        true,
        innerException
    )
    {
    }
}
