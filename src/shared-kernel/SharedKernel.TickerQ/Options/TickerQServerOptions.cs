namespace SharedKernel.TickerQ.Options;

/// <summary>
/// Configuration options for TickerQ server/worker settings.
/// </summary>
public sealed class TickerQServerOptions
{
    /// <summary>
    /// Whether the TickerQ server is enabled for this instance.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Maximum number of concurrent job executions.
    /// </summary>
    public int MaxConcurrency { get; set; } = 4;

    /// <summary>
    /// Default number of retry attempts for failed jobs.
    /// </summary>
    public int DefaultRetries { get; set; } = 3;

    /// <summary>
    /// Default retry intervals in seconds.
    /// </summary>
    public int[] DefaultRetryIntervalsInSeconds { get; set; } = [60, 120, 300];

    /// <summary>
    /// Polling interval in seconds for checking scheduled jobs.
    /// </summary>
    public int? PollingIntervalInSeconds { get; set; }

    /// <summary>
    /// Name for the bootstrap distributed lock (used during job registration).
    /// </summary>
    public string BootstrapLockName { get; set; } = "tickerq:bootstrap";

    /// <summary>
    /// Timeout in seconds for acquiring the bootstrap lock.
    /// </summary>
    public int BootstrapLockTimeoutInSeconds { get; set; } = 180;
}
