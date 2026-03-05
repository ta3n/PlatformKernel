namespace Liberty.Media.Options;

/// <summary>
/// Represents the configuration options for accessing and interacting with AWS S3 storage.
/// </summary>
/// <remarks>
/// This class provides properties for AWS S3 connection settings, such as credentials,
/// region, bucket configuration, and file handling preferences.
/// </remarks>
public class AwsS3Options
{
    /// <summary>
    /// Gets or sets the AWS region used for S3 operations.
    /// </summary>
    /// <remarks>
    /// This property specifies the AWS region where the S3 bucket is located.
    /// It is used to configure the Amazon S3 client with the appropriate region endpoint.
    /// </remarks>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the access key ID used to authenticate requests to the AWS S3 service.
    /// </summary>
    /// <remarks>
    /// The access key ID is part of the credentials required to interact with AWS S3.
    /// It should be kept secure and configured according to AWS best practices.
    /// </remarks>
    public string? AccessKeyId { get; set; }

    /// <summary>
    /// Gets or sets the secret key used for authenticating with the Amazon S3 service.
    /// </summary>
    /// <remarks>
    /// The SecretAccessKey is part of the credentials required to connect to Amazon S3.
    /// It must be used in conjunction with the AccessKeyId to generate signed requests to the S3 API.
    /// Storing and managing the key securely is critical to ensure confidentiality and prevention
    /// of unauthorized access to your Amazon S3 resources.
    /// </remarks>
    public string? SecretAccessKey { get; set; }

    /// <summary>
    /// Specifies the Cache-Control header value to be applied to S3 objects uploaded to the bucket.
    /// This property can be used to define caching behaviors, including max-age and cache visibility.
    /// </summary>
    public string? CacheControlHeader { get; set; }

    /// <summary>
    /// Gets or sets the name of the S3 bucket utilized for file storage operations.
    /// This property is required for managing files within AWS S3, including uploading,
    /// retrieving, and deleting objects. The bucket name must match the existing
    /// configuration on the AWS S3 service.
    /// </summary>
    public string? BucketName { get; set; }

    /// <summary>
    /// Specifies the allowed file extensions for uploads to AWS S3 storage.
    /// </summary>
    /// <remarks>
    /// This property is used to validate the file type before uploading.
    /// It contains a comma-separated list of permissible file extensions (e.g., ".jpg,.png,.pdf").
    /// If empty or null, no specific extension restrictions are enforced during validation.
    /// </remarks>
    public string? AllowExtensionsFile { get; set; }

    /// <summary>
    /// Represents the maximum allowable file size, in megabytes, for uploads to the S3 bucket.
    /// </summary>
    /// <remarks>
    /// This property is used to enforce a size limit on files being uploaded. Any file exceeding
    /// the specified limit will be rejected. The default value is set to 20 MB.
    /// </remarks>
    public int MaxSize { get; set; } = 20;

    /// <summary>
    /// Gets or sets the default folder path used for saving files.
    /// This path is appended to file keys when interacting with AWS S3 services.
    /// </summary>
    /// <remarks>
    /// If no value is explicitly set, the default path is "Liberty/Media/".
    /// </remarks>
    public string DefaultFolderSave { get; set; } = "Liberty/Media/";

    public int PresignedUrlExpiresInSeconds { get; set; } = 3600; // Default expiration time for presigned URLs in seconds
}
