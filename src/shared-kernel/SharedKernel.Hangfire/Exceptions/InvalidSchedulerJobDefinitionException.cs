namespace SharedKernel.Hangfire.Exceptions;

/// <summary>
/// Represents an invalid scheduler job definition that should not be retried.
/// </summary>
public sealed class InvalidSchedulerJobDefinitionException : NonRetryableSchedulerJobException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSchedulerJobDefinitionException"/> class.
    /// </summary>
    /// <param name="message">The validation failure message.</param>
    public InvalidSchedulerJobDefinitionException(
        string message
    ) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSchedulerJobDefinitionException"/> class.
    /// </summary>
    /// <param name="message">The validation failure message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidSchedulerJobDefinitionException(
        string message,
        Exception innerException
    ) : base(
        message,
        innerException
    )
    {
    }
}
