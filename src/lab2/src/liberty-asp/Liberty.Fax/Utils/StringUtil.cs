namespace Liberty.Fax.Utils;

/// <summary>
/// Provides utility methods for string manipulation.
/// </summary>
public static class StringUtil
{
    /// <summary>
    /// Truncates the input string to a specified maximum length and appends an ellipsis ("...") if truncation occurs.
    /// </summary>
    /// <param name="input">The input string to truncate.</param>
    /// <param name="maxLength">The maximum allowed length of the string, including the ellipsis. Default is 50.</param>
    /// <returns>
    /// The truncated string with an ellipsis if truncation occurs, or the original string if it is within the maximum length.
    /// </returns>
    /// <example>
    /// <code>
    /// string result = "This is a long string".TruncateWithEllipsis(10);
    /// // result: "This is..."
    /// </code>
    /// </example>
    public static string TruncateWithEllipsis(
        this string input,
        int maxLength = 50
    )
    {
        input = TrimAndClean(input);
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        const string ellipsis = "...";

        if (input.Length <= maxLength)
        {
            return input;
        }

        var contentLength = maxLength - ellipsis.Length;
        if (contentLength <= 0)
        {
            return ellipsis;
        }

        return input[..contentLength] + ellipsis;
    }

    /// <summary>
    /// Trims whitespace from the input string and removes control characters.
    /// </summary>
    /// <param name="input">The input string to clean.</param>
    /// <returns>A cleaned string with no leading/trailing whitespace or control characters.</returns>
    private static string TrimAndClean(
        string input
    )
    {
        return new string(
            input.Trim()
                .Where(c => !char.IsControl(c))
                .ToArray()
        );
    }
}
