namespace SharedKernel.Hangfire.Exceptions;

/// <summary>
/// Represents a scheduler job failure that should not be retried by Hangfire.
/// </summary>
public class NonRetryableSchedulerJobException : SchedulerJobException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonRetryableSchedulerJobException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    public NonRetryableSchedulerJobException(
        string message
    ) : base(
        message,
        false
    )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonRetryableSchedulerJobException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="innerException">The inner exception.</param>
    public NonRetryableSchedulerJobException(
        string message,
        Exception innerException
    ) : base(
        message,
        false,
        innerException
    )
    {
    }
}
