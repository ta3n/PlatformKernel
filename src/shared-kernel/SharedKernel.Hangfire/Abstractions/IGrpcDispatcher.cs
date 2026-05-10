using SharedKernel.Hangfire.Models;

namespace SharedKernel.Hangfire.Abstractions;

/// <summary>
/// Dispatches a due scheduler job to a target service.
/// </summary>
public interface IGrpcDispatcher
{
    /// <summary>
    /// Sends the payload and scheduler metadata to the configured target gRPC method.
    /// </summary>
    /// <param name="request">The dispatch request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when dispatch finishes.</returns>
    Task DispatchAsync(
        SchedulerGrpcDispatchRequest request,
        CancellationToken cancellationToken = default
    );
}
