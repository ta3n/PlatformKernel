using Liberty.Media.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Liberty.Media.Endpoints;

/// <summary>
/// Provides an endpoint for removing files from the system.
/// </summary>
public static class FileRemoveEndpoint
{
    /// <summary>
    /// Configures the route for removing files via HTTP DELETE.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> that specifies the application’s routing endpoints.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the file remove endpoint configured.</returns>
    public static IEndpointRouteBuilder MapFileRemoveEndpoint(
        this IEndpointRouteBuilder builder
    )
    {
        builder
            .MapDelete("/files/remove/{fileCode}", RemoveFile)
            .RequireAuthorization()
            .WithName("RemoveFile")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return builder;
    }

    /// <summary>
    /// Handles the removal of a file identified by a specific file code
    /// from the underlying storage mechanism.
    /// </summary>
    /// <param name="fileCode">The unique identifier for the file to be removed.</param>
    /// <param name="awsS3Service">The service providing methods for interacting with the AWS S3 storage.</param>
    /// <returns>An HTTP result indicating the outcome of the operation:
    /// 204 No Content if successful, 404 Not Found if the file does not exist, or
    /// 500 Internal Server Error for unexpected errors.</returns>
    private static async Task<IResult> RemoveFile(
        string fileCode,
        IAwsS3Service awsS3Service
    )
    {
        await awsS3Service.RemoveFileAsync(fileCode);

        return Results.NoContent();
    }
}
