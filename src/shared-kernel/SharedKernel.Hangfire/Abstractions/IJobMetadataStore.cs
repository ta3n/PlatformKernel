using SharedKernel.Hangfire.Models;

namespace SharedKernel.Hangfire.Abstractions;

/// <summary>
/// Stores scheduler job definitions independently from Hangfire storage.
/// </summary>
public interface IJobMetadataStore
{
    /// <summary>
    /// Creates or updates a scheduler job definition.
    /// </summary>
    /// <param name="definition">The job definition.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when the definition is saved.</returns>
    Task UpsertAsync(
        SchedulerJobDefinition definition,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a scheduler job definition by job key.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The job definition, or null when not found.</returns>
    Task<SchedulerJobDefinition?> GetByJobKeyAsync(
        string jobKey,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sets the stored scheduler status for a job.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="status">The new scheduler status.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when the status is saved.</returns>
    Task SetStatusAsync(
        string jobKey,
        SchedulerJobStatus status,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes metadata for a scheduler job.
    /// </summary>
    /// <param name="jobKey">The stable scheduler job key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when metadata is deleted.</returns>
    Task DeleteAsync(
        string jobKey,
        CancellationToken cancellationToken = default
    );
}
