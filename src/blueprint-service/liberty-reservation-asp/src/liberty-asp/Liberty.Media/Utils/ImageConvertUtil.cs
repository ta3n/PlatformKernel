using System.Security.Cryptography;
using System.Text;
using ImageMagick;

namespace Liberty.Media.Utils;

/// <summary>
/// Provides utility methods for image manipulation operations including resizing,
/// generating random keys, and sanitizing image names.
/// </summary>
public static class ImageConvertUtil
{
    /// Resizes an image from the specified input stream and returns the resized image as a stream.
    /// <param name="inputStream">
    /// The input stream containing the image to be resized. If null or empty, a null stream is returned.
    /// </param>
    /// <param name="fileExtension">
    /// The file extension of the image, used to determine the correct encoder for saving the resized image.
    /// </param>
    /// <param name="width">
    /// The desired width of the resized image. Defaults to 500.
    /// </param>
    /// <param name="height">
    /// The desired height of the resized image. Defaults to 500.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while performing the resize operation.
    /// </param>
    /// <returns>
    /// A stream containing the resized image. If the input stream is null or empty, a null stream is returned.
    /// </returns>
    public static async Task<Stream> ResizeAsync(
        Stream? inputStream,
        string fileExtension,
        int width = 500,
        int height = 500,
        CancellationToken cancellationToken = default
    )
    {
        if (inputStream is null || width <= 0 || height <= 0)
        {
            return Stream.Null;
        }

        // Read input into MemoryStream (async)
        var memoryStream = new MemoryStream();

        try
        {
            await inputStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);

            if (memoryStream is { Length: <= 0 })
            {
                return Stream.Null;
            }

            memoryStream.Position = 0;

            var outputStream = new MemoryStream();

            using (var image = new MagickImage(memoryStream))
            {
                cancellationToken.ThrowIfCancellationRequested();

                image.AutoOrient();

                // Resize to exact width x height (ignore aspect ratio like original code)
                var geometry = new MagickGeometry(
                    (uint)width,
                    (uint)height
                ) { IgnoreAspectRatio = true };

                image.FilterType = FilterType.Lanczos; // good quality
                image.Resize(geometry);

                // Light optimization
                image.Strip(); // remove metadata
                image.ColorSpace = ColorSpace.sRGB;

                // Select format based on extension
                var ext = fileExtension.Trim().ToLowerInvariant();
                ConfigureOutputFormat(image, ext);

                await image.WriteAsync(outputStream, cancellationToken);
            }

            outputStream.Position = 0;
            return outputStream;
        }
        finally
        {
            await memoryStream.DisposeAsync().ConfigureAwait(false);
        }

        static void ConfigureOutputFormat(
            MagickImage image,
            string? fileExtension
        )
        {
            // Default (fallback): JPEG quality 85
            image.Format = MagickFormat.Jpeg;
            image.Quality = 85;

            var ext = (fileExtension ?? string.Empty).Trim().ToLowerInvariant();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    image.Format = MagickFormat.Jpeg;
                    image.Quality = 85; // customize if needed
                    break;
                case ".png":
                    image.Format = MagickFormat.Png;
                    image.Settings.SetDefine(MagickFormat.Png, "compression-level", "9");
                    break;
                case ".webp":
                    image.Format = MagickFormat.WebP;
                    image.Quality = 80;
                    // If want lossless:
                    // var webp = new WebPWriteDefines { Lossless = true, Method = 4 };
                    break;
            }
        }
    }

    /// <summary>
    /// Generates a random key in hexadecimal format, appended with the given file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension to append to the generated key. Must include the dot (e.g., ".jpg").</param>
    /// <returns>A string representing the generated random key with the file extension appended.</returns>
    public static string GenerateRandomKey(
        string fileExtension
    )
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[16];
        rng.GetBytes(randomBytes);
        return Convert.ToHexString(randomBytes).ToLower() + fileExtension;
    }

    /// <summary>
    /// Removes diacritic marks and non-ASCII characters from a specified image name string,
    /// leaving only valid ASCII characters.
    /// </summary>
    /// <param name="input">The input image name string to sanitize.</param>
    /// <returns>A sanitized string with diacritics and non-ASCII characters removed.</returns>
    public static string SanitizedImageName(
        string input
    )
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (
            var c in from c in normalizedString
            let unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
            where unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark
            where c <= 127
            select c
        )
        {
            stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }
}
