namespace Liberty.SysIntegrationEvent.Models;

/// <summary>
/// Represents a model that contains information about an image resize operation.
/// </summary>
/// <remarks>
/// This model is utilized to pass the necessary data regarding an image file's original code,
/// its resized version's code with size type, and the corresponding size type identifier.
/// It is commonly serialized and transmitted as part of the messaging infrastructure
/// for handling image resizing tasks.
/// </remarks>
/// <param name="OriginalCode">
/// The identifier for the original image file. This property may be null if no original code is specified.
/// </param>
/// <param name="CodeWithSizeType">
/// The identifier for the resized image file, including the size type as part of the code. This property may be null.
/// </param>
/// <param name="SizeType">
/// The string representation of the size type used for resizing the image. This property may be null.
/// </param>
public record ImageResizeModel(
    string? OriginalCode,
    string? CodeWithSizeType,
    string? SizeType
);
