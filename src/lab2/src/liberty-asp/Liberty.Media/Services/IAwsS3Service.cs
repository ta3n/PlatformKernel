using Liberty.Media.Models.Requests;

namespace Liberty.Media.Services;

/// <summary>
/// Interface defining methods for interacting with AWS S3 file storage.
/// </summary>
public interface IAwsS3Service
{
    /// <summary>
    /// Retrieves a file from AWS S3 storage asynchronously based on the provided key code.
    /// </summary>
    /// <param name="keyCode">The unique key code identifying the file in the S3 bucket.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A tuple containing the response stream of the file, the content type, and the file name.
    /// The response stream may be null if the file is not found.
    /// </returns>
    Task<(Stream? responseStream, string contentType, string fileName)> GetFileAsync(
        string keyCode,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves an image from AWS S3 with optional resizing applied based on the specified size type.
    /// </summary>
    /// <param name="keyCode">The unique identifier or key for the image in AWS S3.</param>
    /// <param name="keyCodeWithSizeType">The key that includes size type information for the image in AWS S3.</param>
    /// <param name="sizeType">The size type for the requested image (e.g., thumbnail, medium). If null, the original image is returned.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing the response stream of the image and its content type. The stream is null if the image could not be retrieved.</returns>
    Task<(Stream? responseStream, string contentType)> GetImageWithResizeAsync(
        string keyCode,
        string keyCodeWithSizeType,
        string? sizeType,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously uploads a file to the AWS S3 bucket based on the details provided in the request.
    /// </summary>
    /// <param name="request">The request containing the file to upload and related metadata such as code, file type, and size type.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the operation.</param>
    /// <returns>A string representing the key or identifier for the uploaded file in the S3 bucket.</returns>
    Task<string> UploadFileAsync(
        UploadFileRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Uploads an image to an S3 bucket with the specified key code, size type, and metadata, and returns the generated code and the uploaded image stream.
    /// </summary>
    /// <param name="keyCode">The unique identifier for the image being uploaded.</param>
    /// <param name="sizeType">The size type of the image, which can be null.</param>
    /// <param name="imageName">The name of the image file to be uploaded.</param>
    /// <param name="contentType">The MIME type of the image.</param>
    /// <param name="imageStream">The stream containing the image data to be uploaded.</param>
    /// <param name="cancellationToken">The cancellation token to observe for stopping the operation.</param>
    /// <returns>A tuple containing the generated code for the uploaded image and its stream. The stream may be null if the upload fails.</returns>
    Task<(string code, Stream? responseStream)> UploadImageAsync(
        string keyCode,
        string? sizeType,
        string imageName,
        string contentType,
        Stream imageStream,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes a file asynchronously from the AWS S3 storage based on the provided key code.
    /// </summary>
    /// <param name="keyCode">The unique key code of the file to be removed from S3 storage.</param>
    /// <param name="cancellationToken">A token to observe for cancellation of the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a boolean indicating
    /// whether the file was successfully removed.</returns>
    Task<bool> RemoveFileAsync(
        string keyCode,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks if a specific object exists in the S3 bucket.
    /// </summary>
    /// <param name="keyCode">The key of the S3 object to check for existence.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the S3 object exists (true) or does not exist (false).
    /// </returns>
    Task<bool> DoesS3ObjectExistAsync(
        string keyCode
    );
}
