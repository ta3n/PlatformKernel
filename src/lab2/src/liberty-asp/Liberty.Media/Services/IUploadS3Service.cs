namespace Liberty.Media.Services;

/// <summary>
/// Defines a service responsible for handling file uploads to an Amazon S3 bucket.
/// Includes methods for different upload strategies, optimized for small and large files,
/// as well as for generating pre-signed URLs to facilitate client-side uploads.
/// </summary>
public interface IUploadS3Service
{
    /// <summary>
    /// Uploads a file to an S3 bucket using a suitable method based on the file's characteristics.
    /// </summary>
    /// <param name="keyUpload">The S3 key specifying the target location of the upload.</param>
    /// <param name="sanitizedImageName">The sanitized name of the file to be uploaded.</param>
    /// <param name="contentType">The content type (MIME type) of the uploaded file.</param>
    /// <param name="inputStream">The input stream of the file to be uploaded.</param>
    /// <param name="keyCode">A code used for additional identification or processing of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A tuple containing a code indicating the result of the upload operation and a response stream, which may be null depending on the implementation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any required parameter is null.</exception>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown if an error occurs during the S3 operation.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled by the provided <paramref name="cancellationToken"/>.</exception>
    Task<(string code, Stream? responseStream)> UploadFileAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Uploads a file to an S3 bucket using Amazon S3 Transfer Utility.
    /// </summary>
    /// <param name="keyUpload">The S3 key specifying the target location of the upload.</param>
    /// <param name="sanitizedImageName">The sanitized name of the file to be uploaded.</param>
    /// <param name="contentType">The content type (MIME type) of the uploaded file.</param>
    /// <param name="inputStream">The input stream of the file to be uploaded.</param>
    /// <param name="keyCode">A code used for additional identification or processing of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A tuple containing a code indicating the result of the upload operation and a response stream, which may be null depending on the implementation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any required parameter is null.</exception>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown if an error occurs during the S3 operation.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled by the provided <paramref name="cancellationToken"/>.</exception>
    Task<(string code, Stream? responseStream)> UploadFileWithTransferUtilityAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Uploads a small file to an Amazon S3-compatible storage using the specified parameters.
    /// </summary>
    /// <param name="keyUpload">The key specifying the target storage location for the upload.</param>
    /// <param name="sanitizedImageName">The sanitized name of the file to be uploaded.</param>
    /// <param name="contentType">The MIME content type of the file being uploaded.</param>
    /// <param name="inputStream">The stream containing the file data to be uploaded.</param>
    /// <param name="keyCode">A unique identifier or key associated with the upload operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A tuple containing the unique code of the operation and the response stream from the upload service, which may be null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any required parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the upload operation cannot be completed due to an invalid state.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the provided <paramref name="cancellationToken"/>.</exception>
    Task<(string code, Stream? responseStream)> UploadSmallFileAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Uploads a large file to an S3 bucket using the specified key and input stream.
    /// This method processes large files in chunks to enhance performance and reduce memory consumption.
    /// </summary>
    /// <param name="keyUpload">The S3 key under which the file will be stored.</param>
    /// <param name="sanitizedImageName">The sanitized name of the file to be uploaded.</param>
    /// <param name="contentType">The MIME type of the file to be uploaded.</param>
    /// <param name="inputStream">The input stream containing the file data to be uploaded.</param>
    /// <param name="keyCode">A unique identifier for internal tracking or additional processing of the file.</param>
    /// <param name="cancellationToken">A cancellation token to observe while processing the upload operation.</param>
    /// <returns>
    /// A tuple containing a string code that indicates the result of the upload process and an optional
    /// response stream from the S3 service with additional details.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if any required parameter is null.</exception>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown when an error occurs during the S3 service operation.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    Task<(string code, Stream? responseStream)> UploadLargeFileAsync(
        string keyUpload,
        string sanitizedImageName,
        string contentType,
        Stream inputStream,
        string keyCode,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Generates a pre-signed URL for uploading a file to an Amazon S3 bucket.
    /// </summary>
    /// <param name="key">The S3 key that specifies the target location of the upload.</param>
    /// <param name="contentType">The content type (MIME type) to associate with the uploaded file.</param>
    /// <returns>A pre-signed URL that can be used to upload a file directly to the specified S3 bucket.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="contentType"/> is null or empty.</exception>
    string GenerateUploadUrl(
        string key,
        string contentType
    );
}
