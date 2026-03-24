using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SharedKernel.ElasticSearch.HealthChecks;

/// <summary>
/// Performs a ping-based health check against ElasticSearch.
/// </summary>
public sealed class ElasticSearchHealthCheck(
    Abstractions.IElasticSearchService elasticSearchService
) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var isHealthy = await elasticSearchService.PingAsync(cancellationToken).ConfigureAwait(false);

        return isHealthy
            ? HealthCheckResult.Healthy("ElasticSearch is reachable.")
            : HealthCheckResult.Unhealthy("ElasticSearch ping failed.");
    }
}
