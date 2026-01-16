using Hangfire;

namespace PlatformKernel.Hangfire.Utils;

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
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");

        return new RecurringJobOptions { TimeZone = timeZone };
    }
}
