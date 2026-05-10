using Hangfire.Server;
using SharedKernel.Hangfire.Models;

namespace SharedKernel.Hangfire.Abstractions;

/// <summary>
/// Executes scheduler jobs through metadata lookup and gRPC dispatch.
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// Executes a recurring job occurrence.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="performContext">The Hangfire perform context injected at runtime.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when the occurrence finishes.</returns>
    Task ExecuteRecurringAsync(
        string jobKey,
        PerformContext? performContext = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a specific job occurrence with stable idempotency metadata.
    /// </summary>
    /// <param name="request">The execution request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when execution finishes.</returns>
    Task ExecuteAsync(
        SchedulerJobExecutionRequest request,
        CancellationToken cancellationToken = default
    );
}
