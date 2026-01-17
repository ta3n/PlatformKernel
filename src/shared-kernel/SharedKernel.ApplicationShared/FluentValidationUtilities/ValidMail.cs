using System.Text.RegularExpressions;

namespace SharedKernel.ApplicationShared.FluentValidationUtilities;

/// <summary>
/// Provides utility methods for validating email addresses.
/// </summary>
public static partial class ValidMail
{
    /// <summary>
    /// Validates whether the given email string is in a valid email format.
    /// </summary>
    /// <param name="email">The email string to validate.</param>
    /// <returns>
    /// True if the email string matches the expected email format; otherwise, false.
    /// </returns>
    public static bool BeValidEmail(
        string email
    )
    {
        return MailRegex().IsMatch(email);
    }

    /// Generates a compiled regular expression for validating email addresses.
    /// The regex pattern ensures that the email follows a standard email format with a local part,
    /// an '@' symbol, and a domain part.
    /// <returns>
    /// A compiled Regex instance that can be used to validate email addresses.
    /// </returns>
    [GeneratedRegex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
    private static partial Regex MailRegex();
}
