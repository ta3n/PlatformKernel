namespace SharedKernel.Hangfire.Exceptions;

/// <summary>
/// Base type for scheduler job exceptions that carry retry intent.
/// </summary>
public abstract class SchedulerJobException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerJobException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="retryable">A value indicating whether the failure is retryable.</param>
    protected SchedulerJobException(
        string message,
        bool retryable
    ) : base(message)
    {
        Retryable = retryable;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerJobException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="retryable">A value indicating whether the failure is retryable.</param>
    /// <param name="innerException">The inner exception.</param>
    protected SchedulerJobException(
        string message,
        bool retryable,
        Exception innerException
    ) : base(
        message,
        innerException
    )
    {
        Retryable = retryable;
    }

    /// <summary>
    /// Gets a value indicating whether Hangfire should retry this job failure.
    /// </summary>
    public bool Retryable { get; }
}
