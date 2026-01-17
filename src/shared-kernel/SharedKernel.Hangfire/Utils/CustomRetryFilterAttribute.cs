using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Hangfire.Utils;

/// <summary>
/// Represents a custom retry filter attribute for Hangfire jobs.
/// </summary>
/// <remarks>
/// This attribute is applied to Hangfire jobs to log an error after the job
/// reaches the maximum retry count. It checks if the new job state is a
/// failed state, and if so, evaluates if the retry count has exceeded
/// the maximum allowed retries.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class CustomRetryFilterAttribute(
    ILogger<CustomRetryFilterAttribute> logger
) : JobFilterAttribute, IApplyStateFilter
{
    /// <summary>
    /// Handles the application of a new state to a background job, specifically
    /// verifying the retry count when the new state is a failure.
    /// </summary>
    /// <param name="context">
    /// Provides the state transition context information, including the job details
    /// and the new state to be applied.
    /// </param>
    /// <param name="transaction">
    /// Represents the write-only transaction being carried out during the state
    /// application process.
    /// </param>
    public void OnStateApplied(
        ApplyStateContext context,
        IWriteOnlyTransaction transaction
    )
    {
        if (context.NewState is not FailedState)
        {
            return;
        }

        var retryCount = context.GetJobParameter<int>("RetryCount");
        var maxRetryCount = context.GetJobParameter<int>("MaxRetryCount");

        if (retryCount >= maxRetryCount)
        {
            logger.LogError(
                "Job {JobId} has failed after {RetryCount} retries",
                context.BackgroundJob.Id,
                retryCount
            );
        }
    }

    /// <summary>
    /// Invoked when a state is unapplied from a Hangfire job.
    /// </summary>
    /// <param name="context">Provides context information about the state change including the job and the new state data.</param>
    /// <param name="transaction">Represents a write-only Hangfire transaction.</param>
    public void OnStateUnapplied(
        ApplyStateContext context,
        IWriteOnlyTransaction transaction
    )
    {
        // No action needed when state is unappeased
    }
}
