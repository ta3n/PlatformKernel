using System.Security.Cryptography;

namespace PlatformKernel.ApplicationShared.Utils;

/// <summary>
/// Provides utility methods for generating unique codes, record memos, and notice numbers.
/// </summary>
public static class EntityUtil
{
    /// <summary>
    /// Creates a unique code using a GUID without dashes.
    /// </summary>
    /// <returns>
    /// A string representing a unique code in a 32-character hexadecimal format.
    /// </returns>
    public static string CreateCode()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Creates a record memo based on the current UTC time in Unix timestamp format.
    /// </summary>
    /// <returns>
    /// A string representing the current UTC time as a Unix timestamp.
    /// </returns>
    public static string CreateRecordMemo()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    }

    /// <summary>
    /// Creates a notice number with an optional prefix and a random 3-byte hexadecimal string.
    /// </summary>
    /// <param name="prefix">
    /// An optional string to prepend to the generated notice number. Defaults to an empty string.
    /// </param>
    /// <returns>
    /// A string representing the notice number, consisting of the prefix and a 6-character uppercase hexadecimal string.
    /// </returns>
    /// <example>
    /// <code>
    /// var noticeNumber = EntityUtil.CreateNoticeNumber("XX");
    /// // Example output: "XXFA2BDC"
    /// </code>
    /// </example>
    public static string CreateNoticeNumber(
        string prefix = ""
    )
    {
        // Generate 3 random bytes (3 * 2 = 6 hex chars)
        Span<byte> randomBytes = stackalloc byte[3];
        RandomNumberGenerator.Fill(randomBytes);

        // Convert to uppercase hex string without dashes
        var hex = Convert.ToHexString(randomBytes); // e.g., "FA2BDC"

        return prefix + hex; // e.g., "XXFA2BDC"
    }
}
