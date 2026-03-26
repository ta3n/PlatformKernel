namespace SharedKernel.Hangfire.Options;

/// <summary>
/// Represents Hangfire server and bootstrap coordination options.
/// </summary>
public sealed class HangfireServerOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the local process should host a Hangfire server.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the distributed lock name used to serialize recurring and scheduled job bootstrap.
    /// </summary>
    public string BootstrapLockName { get; set; } = "hangfire:bootstrap";

    /// <summary>
    /// Gets or sets the distributed lock timeout, in seconds, for job bootstrap coordination.
    /// </summary>
    public int BootstrapLockTimeoutInSeconds { get; set; } = 30;
}
