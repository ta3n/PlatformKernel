using Liberty.Media.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Liberty.Media.Endpoints;

/// <summary>
/// Provides an endpoint for handling file preview functionality, enabling clients to preview files
/// retrieved from an AWS S3 bucket.
/// </summary>
public static class FilePreviewEndpoint
{
    /// <summary>
    /// Maps an endpoint to handle file preview requests. Configures the route, handler,
    /// and response types for previewing files stored in an S3-compatible service.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IEndpointRouteBuilder"/> used to configure application routes.
    /// </param>
    /// <returns>
    /// The modified <see cref="IEndpointRouteBuilder"/> with the file preview endpoint added.
    /// </returns>
    public static IEndpointRouteBuilder MapFilePreviewEndpoint(
        this IEndpointRouteBuilder builder
    )
    {
        builder
            .MapGet("/files/preview/{fileCode}", PreviewFile)
            .WithName("PreviewFile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return builder;
    }

    /// <summary>
    /// Retrieves and returns a file stream for a specified file code if it exists in the S3 storage.
    /// </summary>
    /// <param name="fileCode">The unique identifier of the file to retrieve from the S3 storage.</param>
    /// <param name="awsS3Service">The service used for accessing AWS S3 storage to retrieve the file.</param>
    /// <returns>
    /// A result containing the file stream and content type if the file exists, or a 404 Not Found result if the file is not found.
    /// Returns a 500 Internal Server Error result in case of any processing errors.
    /// </returns>
    private static async Task<IResult> PreviewFile(
        [FromRoute] string fileCode,
        IAwsS3Service awsS3Service
    )
    {
        var (responseStream, contentType, _) = await awsS3Service.GetFileAsync(fileCode);

        return responseStream is null
            ? Results.NotFound()
            : Results.File(responseStream, contentType);
    }
}
