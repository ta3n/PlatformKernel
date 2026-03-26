namespace SharedKernel.Hangfire.Options;

/// <summary>
/// Represents Hangfire storage configuration.
/// </summary>
public sealed class HangfireStorageOptions
{
    /// <summary>
    /// Gets or sets the Hangfire storage provider. Supported values are PostgreSql and Redis.
    /// </summary>
    public string Provider { get; set; } = HangfireStorageProviders.PostgreSql;

    /// <summary>
    /// Gets or sets the retention period, in days, for succeeded jobs.
    /// </summary>
    public int SucceededJobExpirationInDays { get; set; } = 7;

    /// <summary>
    /// Gets or sets Redis-specific Hangfire storage options.
    /// </summary>
    public HangfireRedisStorageOptions Redis { get; set; } = new();
}

/// <summary>
/// Contains the supported Hangfire storage provider names.
/// </summary>
public static class HangfireStorageProviders
{
    /// <summary>
    /// PostgreSQL-backed Hangfire storage.
    /// </summary>
    public const string PostgreSql = "PostgreSql";

    /// <summary>
    /// Redis-backed Hangfire storage.
    /// </summary>
    public const string Redis = "Redis";
}
