using Ganss.Xss;

namespace SharedKernel.AppShared.Utils;

/// <summary>
/// Provides utility methods for working with strings.
/// </summary>
public static class StringUtil
{
    /// <summary>
    /// Converts the first character of the given string to uppercase,
    /// while keeping the other characters unchanged.
    /// </summary>
    /// <param name="str">The input string to be modified. Must not be null or empty.</param>
    /// <returns>The modified string with the first character in uppercase.</returns>
    /// <exception cref="ArgumentException">Thrown when the input string is null or empty.</exception>
    public static string ToFirstUpper(
        string str
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(str);

        var array = str.ToCharArray();
        var up = char.ToUpper(array[0]);
        array[0] = up;
        return new string(array);
    }

    /// <summary>
    /// Sanitizes the provided HTML input string to remove any potentially dangerous or unwanted elements and attributes.
    /// </summary>
    /// <param name="input">The HTML input string to sanitize, or null if no input is provided.</param>
    /// <returns>A sanitized version of the input HTML string, or null if the input was null.</returns>
    public static string? HtmlSanitize(
        this string? input
    )
    {
        if (input is null)
        {
            return null;
        }

        var sanitizer = new HtmlSanitizer();
        return sanitizer.Sanitize(input);
    }

    /// Converts the first character of the string to lowercase if it is uppercase.
    /// <param name="str">The input string to convert.</param>
    /// <returns>The input string with the first character converted to lowercase if it was originally uppercase, otherwise the original string.</returns>
    public static string ToCamelCase(
        this string str
    )
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
        {
            return str;
        }

        return char.ToLower(str[0]) + str.Substring(1);
    }
}
