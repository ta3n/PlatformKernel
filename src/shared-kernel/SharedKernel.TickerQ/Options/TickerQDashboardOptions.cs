namespace SharedKernel.TickerQ.Options;

/// <summary>
/// Configuration options for TickerQ dashboard.
/// </summary>
public sealed class TickerQDashboardOptions
{
    /// <summary>
    /// Whether the TickerQ dashboard is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Base path for the dashboard UI (e.g., "tickerq-dashboard").
    /// </summary>
    public string DashboardUrl { get; set; } = "tickerq-dashboard";

    /// <summary>
    /// Whether to enable basic authentication for the dashboard.
    /// </summary>
    public bool EnableBasicAuth { get; set; } = true;

    /// <summary>
    /// Username for basic authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for basic authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Whether the dashboard is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }
}
