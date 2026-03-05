using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Liberty.Fax.Utils;

/// <summary>
/// Utility class providing functionality to handle PDF conversion tasks.
/// </summary>
public static class PdfConverterUtil
{
    /// Converts the given text content into a PDF document and returns its binary data.
    /// <param name="textContent">
    /// The text content to be converted into a PDF document.
    /// </param>
    /// <returns>
    /// A byte array representing the generated PDF document.
    /// </returns>
    public static byte[] ConvertTextToPdf(
        string textContent
    )
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        using var graphics = XGraphics.FromPdfPage(page);

        // Define font and brush
        var font = new XFont("Arial", 12, XFontStyle.Regular);
        var brush = XBrushes.Black;

        // Draw text (supports Japanese characters)
        var rect = new XRect(40, 50, page.Width - 80, page.Height - 100);
        graphics.DrawString(textContent, font, brush, rect, XStringFormats.TopLeft);

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }
}
