using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SharedKernel.ElasticSearch.Options;

/// <summary>
/// Represents health check settings for the ElasticSearch integration.
/// </summary>
public sealed class ElasticSearchHealthCheckOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the ElasticSearch health check is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the health check registration name.
    /// </summary>
    public string Name { get; set; } = "elasticsearch";

    /// <summary>
    /// Gets or sets the failure status returned when the health check fails.
    /// </summary>
    public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;

    /// <summary>
    /// Gets or sets the tags attached to the health check registration.
    /// </summary>
    public string[] Tags { get; set; } = ["ready"];
}
