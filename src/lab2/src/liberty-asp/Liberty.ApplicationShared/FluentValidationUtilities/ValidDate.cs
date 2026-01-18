using System.Globalization;

namespace Liberty.ApplicationShared.FluentValidationUtilities;

/// <summary>
/// Provides static methods to validate various date, color, and time-related values.
/// </summary>
public static class ValidDate
{
    /// <summary>
    /// Validates whether a given date, represented as a long in the format "yyyyMMdd",
    /// can be successfully parsed into a valid <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="date">
    /// A long value representing a date in the format "yyyyMMdd".
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the input is a valid date in the specified format.
    /// </returns>
    public static bool BeValidDate(
        long date
    )
    {
        return DateTime.TryParseExact(
            date.ToString(),
            "yyyyMMdd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _
        );
    }

    /// <summary>
    /// Validates whether the given color string represents a valid color code.
    /// </summary>
    /// <param name="color">
    /// A nullable string representing the color. It is expected to start with '#' and be either 4 or 7 characters long.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the color string is valid.
    /// True if the string starts with '#' and its length is either 4 or 7. Otherwise, false.
    /// </returns>
    public static bool BeValidColor(
        string? color
    )
    {
        return color != null && color.StartsWith('#') && color.Length is 4 or 7;
    }

    /// <summary>
    /// Validates whether the specified <see cref="TimeSpan"/> is a valid time span.
    /// A valid time span is defined as non-negative and less than one day.
    /// If the input is null, the method will treat it as valid.
    /// </summary>
    /// <param name="time">The <see cref="TimeSpan"/> to validate. It can be null.</param>
    /// <returns>
    /// true if the time span is null or lies between 0 (inclusive) and 1 day (exclusive).
    /// Otherwise, returns false.
    /// </returns>
    public static bool BeAValidTimeSpan(
        TimeSpan? time
    )
    {
        if (time is null)
        {
            return true;
        }

        return time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
    }

    /// <summary>
    /// Validates whether the provided string is a valid TimeSpan.
    /// </summary>
    /// <param name="time">The string representation of the time span to validate.</param>
    /// <returns>True if the input string is a valid TimeSpan; otherwise, false.</returns>
    public static bool BeAValidTimeSpan(
        string time
    )
    {
        return TimeSpan.TryParse(
            time,
            CultureInfo.InvariantCulture,
            out _
        );
    }

    /// <summary>
    /// Validates if the given time falls within a valid range from zero to the specified maximum hours.
    /// If the time is null, the method considers it valid.
    /// </summary>
    /// <param name="time">The nullable <see cref="TimeSpan"/> to validate.</param>
    /// <param name="maxHour">The maximum allowable hour as an integer.</param>
    /// <returns>Returns true if the time is null or falls within the range from zero to the specified maximum hours; otherwise, false.</returns>
    public static bool BeAValidWithinLimit(
        TimeSpan? time,
        int maxHour
    )
    {
        if (time is null)
        {
            return true;
        }

        return time >= TimeSpan.Zero && time <= TimeSpan.FromHours(maxHour);
    }

    public static bool BeAValidCheckToDay(
        DateTime? fromDate,
        DateTime? toDate,
        int totalDay
    )
    {
        var fromDateCheck = fromDate?.Date ?? DateTime.UtcNow.AddHours(9).Date;
        var toDateCheck = toDate?.Date ?? DateTime.UtcNow.AddHours(9).Date;
        return fromDateCheck.AddDays(totalDay) >= toDateCheck;
    }
}
