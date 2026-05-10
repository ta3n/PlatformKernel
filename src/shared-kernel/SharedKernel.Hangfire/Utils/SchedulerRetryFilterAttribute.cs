using Hangfire.Common;
using Hangfire.States;

namespace SharedKernel.Hangfire.Utils;

/// <summary>
/// Prevents retry loops for scheduler failures that are explicitly non-retryable.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public sealed class SchedulerRetryFilterAttribute : JobFilterAttribute, IElectStateFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerRetryFilterAttribute"/> class.
    /// </summary>
    public SchedulerRetryFilterAttribute()
    {
        Order = -1;
    }

    /// <summary>
    /// Changes non-retryable failed jobs to a final deleted state before automatic retry runs.
    /// </summary>
    /// <param name="context">The state election context.</param>
    public void OnStateElection(
        ElectStateContext context
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.CandidateState is not FailedState failedState)
        {
            return;
        }

        if (SchedulerRetryPolicy.ShouldRetry(failedState.Exception))
        {
            return;
        }

        context.CandidateState = new DeletedState
        {
            Reason = $"Non-retryable scheduler failure: {failedState.Exception.GetType().Name}"
        };
    }
}
