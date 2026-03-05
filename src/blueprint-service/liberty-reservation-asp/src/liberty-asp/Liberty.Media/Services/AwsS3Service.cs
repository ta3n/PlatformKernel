using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Liberty.Media.Models.Requests;
using Liberty.Media.Options;
using Liberty.Media.Utils;
using Liberty.SysException;
using Liberty.SysException.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Liberty.Media.Services;

public sealed class AwsS3Service : IAwsS3Service, IDisposable
{
    private readonly AmazonS3Client _amazonS3Client;
    private readonly IUploadS3Service _uploadS3Service;
    private readonly IOptions<AwsS3Options> _awsS3Options;
    private readonly ILogger<AwsS3Service> _logger;
    private bool _disposedValue;
    private const int DefaultMaxKbSize = 1048576;

    private static readonly SemaphoreSlim UploadSemaphore = new(10); // Limit 10 concurrent uploads

    public AwsS3Service(
        IUploadS3Service uploadS3Service,
        IOptions<AwsS3Options> awsS3Options,
        ILogger<AwsS3Service> logger
    )
    {
        _uploadS3Service = uploadS3Service;
        _awsS3Options = awsS3Options;
        _logger = logger;

        var region = RegionEndpoint.GetBySystemName(_awsS3Options.Value.Region);

        var config = new AmazonS3Config
        {
            RegionEndpoint = region,
            MaxConnectionsPerServer = 50,
            Timeout = TimeSpan.FromSeconds(30),
            UseHttp = false,
            MaxErrorRetry = 3,
            RetryMode = RequestRetryMode.Adaptive
        };

        _amazonS3Client = new AmazonS3Client(
            _awsS3Options.Value.AccessKeyId,
            _awsS3Options.Value.SecretAccessKey,
            config
        );
    }

    public async Task<(Stream? responseStream, string contentType, string fileName)> GetFileAsync(
        string keyCode,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyCode))
            {
                throw new AppRequestInvalidException(ErrorCode.E0100, keyCode);
            }

            var fullKeyCode = $"{_awsS3Options.Value.DefaultFolderSave}{keyCode}";
            var request = new GetObjectRequest
            {
                BucketName = _awsS3Options.Value.BucketName,
                Key = fullKeyCode
                // ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.None
            };

            var response = await _amazonS3Client.GetObjectAsync(
                request,
                cancellationToken
            );

            if (response.HttpStatusCode is HttpStatusCode.OK)
            {
                return (
                    response.ResponseStream,
                    response.Headers.ContentType,
                    response.Metadata["original-file-name"]
                );
            }

            response.Dispose();
            return default;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
            _logger.LogWarning(
                ex,
                "File not found in S3: {KeyCode}, Error: {Message}",
                keyCode,
                ex.Message
            );

            // throw new AppFileNotfoundException(keyCode);

            return default;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Error retrieving file {KeyCode} from S3: {Message}",
                keyCode,
                ex.Message
            );

            // throw new AppLibertyException(ex.Message, ex);

            return default;
        }
    }

    public async Task<(Stream? responseStream, string contentType)> GetImageWithResizeAsync(
        string keyCode,
        string keyCodeWithSizeType,
        string? sizeType,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(sizeType) || sizeType == nameof(ImageSizeType.Original))
        {
            var (responseStream, contentType, _) = await GetFileAsync(
                keyCode,
                cancellationToken
            );

            return (responseStream, contentType);
        }

        try
        {
            var (responseStream, contentType, _) = await GetFileAsync(
                keyCodeWithSizeType,
                cancellationToken
            );

            return (responseStream, contentType);
        }
        catch
        {
            return (null, string.Empty);
        }
    }

    public async Task<string> UploadFileAsync(
        UploadFileRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var file = request.Attachment
            ?? throw new AppRequestInvalidException(
                ErrorCode.E0100,
                "No file provided or file is empty."
            );

        var (isError, message) = UploadFileValidator(request);

        if (isError)
        {
            throw new AppRequestInvalidException(ErrorCode.E0100, message);
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        await using var inputStream = file.OpenReadStream();
        var processedStream = inputStream;

        if (request is { IsImage: true, SizeType: not null })
        {
            var (width, height) = ImageSizeUtil.GetSizeByType(request.SizeType);
            var resizedStream = await ImageConvertUtil.ResizeAsync(
                inputStream,
                fileExtension,
                width,
                height,
                cancellationToken
            );

            if (resizedStream != inputStream)
            {
                processedStream = resizedStream;
            }
        }

        try
        {
            var (code, _) = await UploadImageAsync(
                request.Code,
                request.SizeType,
                file.FileName,
                file.ContentType,
                inputStream,
                cancellationToken
            );

            return code;
        }
        finally
        {
            if (processedStream != inputStream)
            {
                await processedStream.DisposeAsync();
            }
        }
    }

    public async Task<(string code, Stream? responseStream)> UploadImageAsync(
        string keyCode,
        string? sizeType,
        string imageName,
        string contentType,
        Stream imageStream,
        CancellationToken cancellationToken = default
    )
    {
        if (!await UploadSemaphore.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken))
        {
            throw new TooManyConcurrentUploadException();
        }

        var inputStream = imageStream;

        try
        {
            if (sizeType is not null)
            {
                var (width, height) = ImageSizeUtil.GetSizeByType(sizeType);
                inputStream = await ImageConvertUtil.ResizeAsync(
                    imageStream,
                    contentType,
                    width,
                    height,
                    cancellationToken
                );
            }

            var keyUpload = $"{_awsS3Options.Value.DefaultFolderSave}{keyCode}";
            var sanitizedImageName = ImageConvertUtil.SanitizedImageName(imageName);

            var (code, responseStream) = await _uploadS3Service.UploadFileAsync(
                keyUpload,
                sanitizedImageName,
                contentType,
                inputStream,
                keyCode,
                cancellationToken
            );

            // Dispose stream response
            if (responseStream != null)
            {
                await responseStream.DisposeAsync();
            }

            return (code, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error uploading file {KeyCode} ({SizeType}) to S3: {Message}",
                keyCode,
                sizeType ?? "Original",
                ex.Message
            );
            throw new AppLibertyException(ex.Message, ex);
        }
        finally
        {
            await inputStream.DisposeAsync();
            UploadSemaphore.Release();
        }
    }

    public async Task<bool> RemoveFileAsync(
        string keyCode,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyCode))
            {
                throw new AppRequestInvalidException(ErrorCode.E0100, keyCode);
            }

            var fullKeyCode = $"{_awsS3Options.Value.DefaultFolderSave}{keyCode}";
            var requestExits = new GetObjectRequest
            {
                BucketName = _awsS3Options.Value.BucketName,
                Key = fullKeyCode
            };

            var responseExits = await _amazonS3Client.GetObjectAsync(requestExits, cancellationToken);

            if (responseExits is not { HttpStatusCode: HttpStatusCode.OK })
            {
                throw new AppRequestInvalidException(ErrorCode.E0100, keyCode);
            }

            var requestDelete = new DeleteObjectRequest
            {
                BucketName = _awsS3Options.Value.BucketName,
                Key = fullKeyCode
            };

            var responseDelete = await _amazonS3Client.DeleteObjectAsync(requestDelete, cancellationToken);

            return responseDelete.HttpStatusCode is HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error delete file {KeyCode} in the S3: {Message}", keyCode, ex.Message);

            return false;
        }
    }

    public async Task<bool> DoesS3ObjectExistAsync(
        string keyCode
    )
    {
        try
        {
            await _amazonS3Client.GetObjectMetadataAsync(
                _awsS3Options.Value.BucketName,
                keyCode
            );
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void Dispose(
        bool disposing
    )
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _amazonS3Client.Dispose();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private (bool isError, string messsage) UploadFileValidator(
        UploadFileRequest request
    )
    {
        var file = request.Attachment;
        if (file is null || file.Length == 0)
        {
            return (true, "No file provided or file is empty.");
        }

        var allowedExtensions = _awsS3Options.Value.AllowExtensionsFile;
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (request.IsImage
            && !string.IsNullOrWhiteSpace(allowedExtensions)
            && !allowedExtensions.Contains(fileExtension))
        {
            return (true, $"Invalid file type {allowedExtensions} files are allowed.");
        }

        var maxSize = _awsS3Options.Value.MaxSize;
        if (file.Length > maxSize * DefaultMaxKbSize)
        {
            return (
                true,
                $"File size exceeds the maximum allowed size of {maxSize} MB."
            );
        }

        return (false, string.Empty);
    }
}
