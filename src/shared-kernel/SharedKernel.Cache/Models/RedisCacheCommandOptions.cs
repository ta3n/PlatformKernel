namespace SharedKernel.Cache.Models;

/// <summary>
/// Represents per-command Redis options for raw Redis operations.
/// </summary>
public sealed record RedisCacheCommandOptions
{
    /// <summary>
    /// Gets or sets the Redis database index for the command.
    /// </summary>
    public int? Database { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the configured instance prefix should be ignored.
    /// </summary>
    public bool IgnoreInstanceName { get; init; }

    /// <summary>
    /// Gets or sets the expiration time applied to the command when relevant.
    /// </summary>
    public TimeSpan? Expiry { get; init; }
}
