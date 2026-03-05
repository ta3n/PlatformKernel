namespace Liberty.ApplicationShared.Utils;

public static class FileSizeConverter
{
    // Define unit constants
    public const long Byte = 1;
    public const long Kb = 1024;
    public const long Mb = 1024 * 1024;
    public const long Gb = 1024 * 1024 * 1024;

    // Define size threshold constants
    public const long Size512Kb = 512 * Kb;
    public const long Size1Mb = 1 * Mb;
    public const long Size2Mb = 2 * Mb;
    public const long Size5Mb = 5 * Mb;
    public const long Size10Mb = 10 * Mb;

    /// <summary>
    /// Converts a number from scientific notation to human-readable format.
    /// </summary>
    /// <param name="scientificNotation">Value in scientific notation (Example: 6.968901e+06)</param>
    /// <returns>String in human-readable format (Example: 6.64 MB)</returns>
    public static string ConvertFromScientific(
        string scientificNotation
    )
    {
        if (!double.TryParse(scientificNotation, out var value))
        {
            return "Invalid format";
        }

        var bytes = (long)value;
        return ToHumanReadable(bytes);
    }

    /// <summary>
    /// Converts bytes to a human-readable string with appropriate unit (KB, MB, GB).
    /// </summary>
    /// <param name="bytes">File size in bytes</param>
    /// <returns>Human-readable string with unit</returns>
    public static string ToHumanReadable(
        long bytes
    )
    {
        return bytes switch
        {
            >= Gb => $"{bytes / (double)Gb:F2} GB",
            >= Mb => $"{bytes / (double)Mb:F2} MB",
            >= Kb => $"{bytes / (double)Kb:F2} KB",
            _ => $"{bytes} bytes"
        };
    }

    /// <summary>
    /// Converts bytes to kilobytes (KB).
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Size in KB</returns>
    public static double ToKilobytes(
        long bytes
    )
    {
        return (double)bytes / Kb;
    }

    /// <summary>
    /// Converts bytes to megabytes (MB).
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Size in MB</returns>
    public static double ToMegabytes(
        long bytes
    )
    {
        return (double)bytes / Mb;
    }

    /// <summary>
    /// Converts bytes to gigabytes (GB).
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Size in GB</returns>
    public static double ToGigabytes(
        long bytes
    )
    {
        return (double)bytes / Gb;
    }

    /// <summary>
    /// Checks if the file size is small (≤ 512KB)
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>True if size ≤ 512KB</returns>
    public static bool IsSmallFile(
        long bytes
    )
    {
        return bytes <= Size512Kb;
    }

    /// <summary>
    /// Checks if the file size is medium (> 512KB and ≤ 2MB)
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>True if size > 512KB and ≤ 2MB</returns>
    public static bool IsMediumFile(
        long bytes
    )
    {
        return bytes is > Size512Kb and <= Size2Mb;
    }

    /// <summary>
    /// Checks if the file size is large (> 2MB)
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>True if size > 2MB</returns>
    public static bool IsLargeFile(
        long bytes
    )
    {
        return bytes > Size2Mb;
    }
}
