using Liberty.Media.Models.Requests;
using Liberty.Media.Services;

namespace Liberty.Reservation.User.File.WebAPI.Test.InfrastructureOfTest;

public class MockAwsS3Service : IAwsS3Service
{
    public Task<(Stream? responseStream, string contentType, string fileName)> GetFileAsync(
        string keyCode,
        CancellationToken cancellationToken = default
    )
    {
        var (imageStream, contentType) = CreateMockJpegMemoryStream();
        return Task.FromResult<(Stream? responseStream, string contentType, string fileName)>(
            (imageStream, contentType, string.Empty)
        );
    }

    public Task<(Stream? responseStream, string contentType)> GetImageWithResizeAsync(
        string keyCode,
        string keyCodeWithSizeType,
        string? sizeType,
        CancellationToken cancellationToken = default
    )
    {
        var (imageStream, contentType) = CreateMockJpegMemoryStream();
        return Task.FromResult<(Stream? responseStream, string contentType)>(
            (imageStream, contentType)
        );
    }

    public Task<string> UploadFileAsync(
        UploadFileRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Guid.NewGuid().ToString("N"));
    }

    public Task<(string code, Stream? responseStream)> UploadImageAsync(
        string keyCode,
        string? sizeType,
        string imageName,
        string contentType,
        Stream imageStream,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult<(string code, Stream? responseStream)>(
            (Guid.NewGuid().ToString("N"), new MemoryStream())
        );
    }

    public Task<bool> RemoveFileAsync(
        string keyCode,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(true);
    }

    public Task<bool> DoesS3ObjectExistAsync(
        string keyCode
    )
    {
        return Task.FromResult(false);
    }

    private static (Stream ImageStream, string ContentType) CreateMockJpegMemoryStream()
    {
        // A sample JPEG header byte array (not a full image, just for demonstration).
        // Replace with an actual JPEG byte array for real tests.
        var jpegBytes = new byte[]
        {
            0xFF,
            0xD8,
            0xFF,
            0xE0,
            0x00,
            0x10,
            0x4A,
            0x46,
            0x49,
            0x46,
            0x00,
            0x01,
            0x01,
            0x01,
            0x00,
            0x60,
            0x00,
            0x60,
            0x00,
            0x00,
            0xFF,
            0xDB,
            0x00,
            0x43,
            0x00,
            0x08,
            0x06,
            0x06,
            0x07,
            0x06,
            0x05,
            0x08,
            // The rest of the JPEG byte array goes here...
            0xFF,
            0xD9 // JPEG end marker
        };

        // Load the byte array into a MemoryStream
        return (
            new MemoryStream(jpegBytes),
            "image/jpeg"
        );
    }
}
