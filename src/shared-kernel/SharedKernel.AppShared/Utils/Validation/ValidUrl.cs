using System.Text.RegularExpressions;

namespace SharedKernel.AppShared.Utils.Validation;

/// <summary>
/// Provides utility methods for URL validation.
/// </summary>
public static partial class ValidUrl
{
    /// <summary>
    /// A compiled regular expression that matches a valid hostname within a URL.
    /// </summary>
    /// <returns>
    /// A <see cref="System.Text.RegularExpressions.Regex"/> instance representing the compiled regular expression for hostname validation.
    /// </returns>
    [GeneratedRegex(@"(([www\.])?|([\da-z-\.]+))\.([a-z\.]{2,})$")]
    private static partial Regex HostNameRegex();

    /// <summary>
    /// Validates whether the given URL is a well-formed absolute URL
    /// with either an HTTP or HTTPS scheme and a valid host name based on the defined regular expression.
    /// </summary>
    /// <param name="url">The URL string to validate, which can be null.</param>
    /// <returns>True if the input URL is a valid absolute URL with an HTTP or HTTPS scheme and a valid hostname, otherwise false.</returns>
    public static bool BeValidUrl(
        string? url
    )
    {
        return Uri.TryCreate(
                url,
                UriKind.Absolute,
                out var result
            )
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)
            && HostNameRegex().IsMatch(result.Host);
    }
}
