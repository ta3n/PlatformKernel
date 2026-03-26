using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace SharedKernel.Hangfire.Utils;

/// <summary>
/// Applies a retention window to succeeded Hangfire jobs so they expire automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public sealed class SucceededJobExpirationFilterAttribute(
    TimeSpan expirationTimeout
) : JobFilterAttribute, IApplyStateFilter
{
    /// <summary>
    /// Gets the configured succeeded-job expiration timeout.
    /// </summary>
    public TimeSpan ExpirationTimeout { get; } = expirationTimeout > TimeSpan.Zero
        ? expirationTimeout
        : throw new ArgumentOutOfRangeException(
            nameof(expirationTimeout),
            "Succeeded job expiration timeout must be greater than zero."
        );

    /// <summary>
    /// Applies the configured expiration timeout when a job enters the succeeded state.
    /// </summary>
    /// <param name="context">The Hangfire state transition context.</param>
    /// <param name="transaction">The Hangfire write transaction.</param>
    public void OnStateApplied(
        ApplyStateContext context,
        IWriteOnlyTransaction transaction
    )
    {
        if (context.NewState is SucceededState)
        {
            context.JobExpirationTimeout = ExpirationTimeout;
        }
    }

    /// <summary>
    /// Invoked when a state is unapplied from a Hangfire job.
    /// </summary>
    /// <param name="context">The Hangfire state transition context.</param>
    /// <param name="transaction">The Hangfire write transaction.</param>
    public void OnStateUnapplied(
        ApplyStateContext context,
        IWriteOnlyTransaction transaction
    )
    {
        // No action needed when a state is removed.
    }
}
