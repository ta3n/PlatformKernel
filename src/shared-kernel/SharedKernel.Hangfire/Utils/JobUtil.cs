using Hangfire;

namespace SharedKernel.Hangfire.Utils;

/// <summary>
/// Provides utility methods and constants for managing Hangfire jobs,
/// including configurations for recurring jobs and default time zones.
/// </summary>
public static class JobUtil
{
    /// <summary>
    /// Represents the default time zone used for scheduling recurring jobs.
    /// The default value is set to "Asia/Tokyo".
    /// </summary>
    public const string DefaultTimeZone = "Asia/Tokyo";

    /// Retrieves the default recurring job options with pre-configured settings.
    /// This method creates and returns an instance of RecurringJobOptions, pre-populated with default settings for
    /// scheduling jobs. It sets the time zone to "Asia/Tokyo" as the default time zone for the recurring job.
    /// <returns>
    /// An instance of RecurringJobOptions with default settings applied.
    /// </returns>
    public static RecurringJobOptions GetDefaultRecurringJobOptions()
    {
        return GetRecurringJobOptions(DefaultTimeZone);
    }

    /// <summary>
    /// Retrieves recurring job options for the specified time zone.
    /// </summary>
    /// <param name="timeZoneId">The time zone id. When omitted, the default time zone is used.</param>
    /// <returns>An instance of <see cref="RecurringJobOptions"/> with a configured time zone.</returns>
    public static RecurringJobOptions GetRecurringJobOptions(
        string? timeZoneId
    )
    {
        var resolvedTimeZoneId = string.IsNullOrWhiteSpace(timeZoneId)
            ? DefaultTimeZone
            : timeZoneId;
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(resolvedTimeZoneId);

        return new RecurringJobOptions { TimeZone = timeZone };
    }
}
