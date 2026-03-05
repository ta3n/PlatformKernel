using System.Buffers;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Liberty.Media.Options;
using Microsoft.Extensions.Options;

namespace Liberty.Media.Services;

public class UploadS3Service : IUploadS3Service
{
    private readonly AmazonS3Client _amazonS3Client;
    private readonly IOptions<AwsS3Options> _awsS3Options;

    public UploadS3Service(
        IOptions<AwsS3Options> awsS3Options
    )
    {
        _awsS3Options = awsS3Options;

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

    public async Task<(string code, Stream? responseStream)> UploadFileAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    )
    {
        var (code, responseStream) = await UploadFileWithTransferUtilityAsync(
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

        // const long multipartThreshold = 5 * 1024 * 1024; // 5MB
        // if (inputStream.Length > multipartThreshold)
        // {
        //     var (code, responseStream) = await _uploadS3Service.UploadLargeFileAsync(
        //         keyUpload,
        //         sanitizedImageName,
        //         contentType,
        //         inputStream,
        //         keyCode,
        //         cancellationToken
        //     );
        //
        //     // Dispose stream response
        //     if (responseStream != null)
        //     {
        //         await responseStream.DisposeAsync();
        //     }
        //
        //     return (code, null);
        // }
        //
        // var smallResult = await _uploadS3Service.UploadSmallFileAsync(
        //     keyUpload,
        //     sanitizedImageName,
        //     contentType,
        //     inputStream,
        //     keyCode,
        //     cancellationToken
        // );
        //
        // // Dispose stream response
        // if (smallResult.responseStream != null)
        // {
        //     await smallResult.responseStream.DisposeAsync();
        // }
        //
        // return (smallResult.code, null)
    }

    public async Task<(string code, Stream? responseStream)> UploadFileWithTransferUtilityAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    )
    {
        var transferUtilityConfig = new TransferUtilityConfig { MinSizeBeforePartUpload = 7 * 1024 * 1024 };

        using var transferUtility = new TransferUtility(
            _amazonS3Client,
            transferUtilityConfig
        );

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = inputStream,
            Key = keyUpload,
            BucketName = _awsS3Options.Value.BucketName,
            ContentType = contentType,
            AutoCloseStream = false,
            AutoResetStreamPosition = false,
            CannedACL = S3CannedACL.Private,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            DisablePayloadSigning = true,
            DisableMD5Stream = true
        };

        uploadRequest.Metadata.Add("original-file-name", sanitizedImageName);
        uploadRequest.Headers.CacheControl = _awsS3Options.Value.CacheControlHeader;

        await transferUtility.UploadAsync(uploadRequest, cancellationToken);

        return (keyCode, null);
    }

    public async Task<(string code, Stream? responseStream)> UploadSmallFileAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    )
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _awsS3Options.Value.BucketName,
            Key = keyUpload,
            InputStream = inputStream,
            ContentType = contentType,
            Metadata = { ["original-file-name"] = sanitizedImageName },
            Headers = { CacheControl = _awsS3Options.Value.CacheControlHeader },
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            AutoCloseStream = false,
            AutoResetStreamPosition = false,
            DisablePayloadSigning = true,
            DisableMD5Stream = true
        };

        var response = await _amazonS3Client.PutObjectAsync(
            putObjectRequest,
            cancellationToken
        );
        inputStream.Position = 0;

        return (
            response.HttpStatusCode == HttpStatusCode.OK ? keyCode : string.Empty,
            null
        );
    }

    public async Task<(string code, Stream? responseStream)> UploadLargeFileAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    )
    {
        var bufferPool = ArrayPool<byte>.Shared;
        const int partSize = 5 * 1024 * 1024;

        var initiateRequest = new InitiateMultipartUploadRequest
        {
            BucketName = _awsS3Options.Value.BucketName,
            Key = keyUpload,
            ContentType = contentType,
            Metadata = { ["original-file-name"] = sanitizedImageName },
            Headers = { CacheControl = _awsS3Options.Value.CacheControlHeader },
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
        };

        var initiateResponse = await _amazonS3Client.InitiateMultipartUploadAsync(
            initiateRequest,
            cancellationToken
        );
        var uploadId = initiateResponse.UploadId;

        try
        {
            var partETags = new List<PartETag>();
            var partNumber = 1;
            var buffer = bufferPool.Rent(partSize);

            try
            {
                while (true)
                {
                    var bytesRead = await inputStream.ReadAsync(
                        buffer.AsMemory(0, partSize),
                        cancellationToken
                    );
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    await using var partStream = new MemoryStream(buffer, 0, bytesRead, false);

                    var uploadPartRequest = new UploadPartRequest
                    {
                        BucketName = _awsS3Options.Value.BucketName,
                        Key = keyUpload,
                        UploadId = uploadId,
                        PartNumber = partNumber,
                        InputStream = partStream,
                        PartSize = bytesRead,
                        DisablePayloadSigning = true,
                        DisableMD5Stream = true
                    };

                    var uploadPartResponse = await _amazonS3Client.UploadPartAsync(
                        uploadPartRequest,
                        cancellationToken
                    );
                    partETags.Add(new PartETag(partNumber, uploadPartResponse.ETag));
                    partNumber++;
                }
            }
            finally
            {
                bufferPool.Return(buffer);
            }

            var completeRequest = new CompleteMultipartUploadRequest
            {
                BucketName = _awsS3Options.Value.BucketName,
                Key = keyUpload,
                UploadId = uploadId,
                PartETags = partETags
            };

            await _amazonS3Client.CompleteMultipartUploadAsync(
                completeRequest,
                cancellationToken
            );

            return (keyCode, null);
        }
        catch (Exception)
        {
            await _amazonS3Client.AbortMultipartUploadAsync(
                new AbortMultipartUploadRequest
                {
                    BucketName = _awsS3Options.Value.BucketName,
                    Key = keyUpload,
                    UploadId = uploadId
                },
                cancellationToken
            );
            throw;
        }
    }

    public string GenerateUploadUrl(
        string key,
        string contentType
    )
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _awsS3Options.Value.BucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddSeconds(
                _awsS3Options.Value.PresignedUrlExpiresInSeconds
            ),
            ContentType = contentType
        };

        return _amazonS3Client.GetPreSignedURL(request);
    }
}
