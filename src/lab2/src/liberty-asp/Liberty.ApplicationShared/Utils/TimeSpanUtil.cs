namespace Liberty.ApplicationShared.Utils;

/// <summary>
/// Provides utility methods for working with TimeSpan objects.
/// </summary>
public static class TimeSpanUtil
{
    /// <summary>
    /// Converts the given TimeSpan into a formatted string representing the total hours and minutes.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to convert to hour and minute format.</param>
    /// <returns>A string representing the TimeSpan in "hours:minutes" format.</returns>
    public static string To24HourFormat(
        this TimeSpan? timeSpan
    )
    {
        var ts = timeSpan ?? TimeSpan.Zero;
        var totalHours = (int)ts.TotalHours;
        return $"{totalHours}:{ts.Minutes:D2}";
    }
}
