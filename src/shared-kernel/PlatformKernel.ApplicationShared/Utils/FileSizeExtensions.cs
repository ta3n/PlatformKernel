namespace PlatformKernel.ApplicationShared.Utils;

/// <summary>
/// Extension methods for easy conversion of bytes to other units
/// </summary>
public static class FileSizeExtensions
{
    /// <summary>
    /// Converts bytes to kilobytes (KB).
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Size in KB</returns>
    public static double ToKb(
        this long bytes
    )
    {
        return FileSizeConverter.ToKilobytes(bytes);
    }

    /// <summary>
    /// Converts bytes to megabytes (MB).
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Size in MB</returns>
    public static double ToMb(
        this long bytes
    )
    {
        return FileSizeConverter.ToMegabytes(bytes);
    }

    /// <summary>
    /// Converts bytes to gigabytes (GB).
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Size in GB</returns>
    public static double ToGb(
        this long bytes
    )
    {
        return FileSizeConverter.ToGigabytes(bytes);
    }

    /// <summary>
    /// Converts scientific notation to a human-readable format.
    /// </summary>
    /// <param name="scientificNotation">Value in scientific notation</param>
    /// <returns>Human-readable string</returns>
    public static string ToFileSize(
        this string scientificNotation
    )
    {
        return FileSizeConverter.ConvertFromScientific(scientificNotation);
    }
}
