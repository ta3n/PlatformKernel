using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Liberty.Media.Models.Requests;

/// <summary>
/// A request model for uploading a file to a storage service or endpoint.
/// </summary>
/// <remarks>
/// This record encapsulates necessary data for file upload operations, including the file itself,
/// associated metadata, and optional configurations.
/// </remarks>
public record UploadFileRequest(
    IFormFile? Attachment,
    [FromForm] string Code,
    [FromForm] bool IsImage,
    [FromForm] string? SizeType
);
