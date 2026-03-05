namespace Liberty.Media.Utils;

/// <summary>
/// Provides utility methods for handling image size types
/// and retrieving corresponding width and height dimensions.
/// </summary>
public static class ImageSizeUtil
{
    /// <summary>
    /// Retrieves the width and height of an image based on the specified size type.
    /// </summary>
    /// <param name="sizeType">The size type string corresponding to predefined size values (e.g., "Small", "Medium", etc.).</param>
    /// <returns>A tuple containing the width and height of the image in pixels.</returns>
    public static (int width, int height) GetSizeByType(
        string? sizeType
    )
    {
        return sizeType switch
        {
            nameof(ImageSizeType.Small) => (100, 100),
            nameof(ImageSizeType.Medium) => (500, 500),
            nameof(ImageSizeType.Large) => (1000, 1000),
            nameof(ImageSizeType.Compress50) => (50, 50),
            nameof(ImageSizeType.Compress100) => (100, 100),
            nameof(ImageSizeType.Compress150) => (150, 150),
            nameof(ImageSizeType.Compress300) => (300, 300),
            nameof(ImageSizeType.Compress500) => (500, 500),
            nameof(ImageSizeType.Compress1024) => (1024, 1024),
            nameof(ImageSizeType.Compress400X300) => (400, 300),
            nameof(ImageSizeType.Compress800X600) => (800, 600),
            nameof(ImageSizeType.Compress1600X1200) => (1600, 1200),
            _ => (500, 500)
        };
    }
}

/// <summary>
/// Represents the various size types available for images.
/// </summary>
/// <remarks>
/// The enum defines predefined dimensions for resizing images. It includes standard sizes for thumbnails,
/// medium and large images, as well as compressed options with specific resolutions.
/// </remarks>
public enum ImageSizeType
{
    /// <summary>
    /// Represents a small image size.
    /// </summary>
    /// <remarks>
    /// The <c>Small</c> value of the <see cref="ImageSizeType"/> enum is used to indicate
    /// a predefined size of 100x100 pixels for an image. This size may be utilized in scenarios
    /// where compact image representations are required, such as in thumbnails or previews.
    /// </remarks>
    Small,

    /// <summary>
    /// Represents a medium image size type.
    /// </summary>
    /// <remarks>
    /// This enumeration member specifies a standard size of 500x500 pixels.
    /// It is commonly used for medium-sized image requirements.
    /// </remarks>
    Medium,

    /// <summary>
    /// Represents a large image size.
    /// </summary>
    /// <remarks>
    /// The <c>Large</c> value of the <see cref="ImageSizeType"/> enum is used to indicate
    /// a predefined size of 1024x768 pixels for an image. This size is typically suitable
    /// for use in contexts requiring high-resolution visuals, such as detailed image displays
    /// or large content areas.
    /// </remarks>
    Large,

    /// <summary>
    /// Represents the original size of an image without any resizing or compression.
    /// This value is used to fetch the image in its unmodified, full-size form.
    /// </summary>
    Original,

    /// <summary>
    /// Represents the "Compress50" image size type, typically used for resized or compressed images.
    /// This size type corresponds to an image dimension of 50x50 pixels.
    /// </summary>
    Compress50,

    /// <summary>
    /// Represents an image size type for compressing images to 100x100 pixels.
    /// </summary>
    /// <remarks>
    /// This enum value is used for resizing images while maintaining a square aspect ratio
    /// with dimensions of 100x100 pixels. Typically used in scenarios where
    /// reduced image sizes are required for optimization or specific display requirements.
    /// </remarks>
    Compress100,

    /// <summary>
    /// Represents an image size type that corresponds to a compressed dimension of 150x150 pixels.
    /// </summary>
    /// <remarks>
    /// This enum member is used to specify the target dimensions when resizing or compressing images
    /// to a size of 150x150 pixels. Often utilized in applications to optimize image storage or
    /// display where a medium-small size is required.
    /// </remarks>
    Compress150,

    /// <summary>
    /// Represents a compressed image size with a predefined quality of 300.
    /// </summary>
    /// <remarks>
    /// The <c>Compress300</c> value of the <see cref="ImageSizeType"/> enum denotes an image size configuration
    /// with specific compression settings optimized for a balance between quality and file size. It can be used
    /// for applications requiring medium-quality compressed images while maintaining a reduced file footprint.
    /// </remarks>
    Compress300,

    /// <summary>
    /// Represents an image size type for a compressed image with dimensions of 500x500 pixels.
    /// This enum value can be used to retrieve or process an image resized to these specific dimensions.
    /// </summary>
    Compress500,

    /// <summary>
    /// Represents an image size type with dimensions of 1024x1024 pixels.
    /// This size type is commonly used for compressing images into a standard resolution
    /// while maintaining clarity and reducing file size.
    /// </summary>
    Compress1024,

    /// <summary>
    /// Represents a specific image size type with dimensions of 400 pixels in width and 300 pixels in height.
    /// This size is typically used for compressing images to these exact dimensions while maintaining aspect ratio,
    /// or for scenarios requiring smaller image formats with specific proportions such as 4:3.
    /// </summary>
    Compress400X300,

    /// <summary>
    /// Represents an image size type for compression with a resolution of 800x600 pixels.
    /// </summary>
    Compress800X600,

    /// <summary>
    /// Represents an image size with dimensions 1600x1200 pixels, used for compressed versions of images.
    /// </summary>
    Compress1600X1200
}
