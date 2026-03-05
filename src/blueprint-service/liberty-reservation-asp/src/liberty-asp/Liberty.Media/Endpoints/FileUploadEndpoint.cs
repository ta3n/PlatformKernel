using Liberty.Media.Models.Requests;
using Liberty.Media.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Liberty.Media.Endpoints;

/// <summary>
/// Provides an endpoint for uploading files to the server.
/// </summary>
public static class FileUploadEndpoint
{
    /// Maps the file upload endpoint to the provided endpoint route builder. This includes setting up the
    /// route for handling file uploads, applying necessary configurations such as authorization, and
    /// specifying response types for different status codes.
    /// <param name="builder">The IEndpointRouteBuilder to which the file upload endpoint should be mapped.</param>
    /// <returns>The IEndpointRouteBuilder with the file upload endpoint configured.</returns>
    public static IEndpointRouteBuilder MapFileUploadEndpoint(
        this IEndpointRouteBuilder builder
    )
    {
        builder
            .MapPost("/files/upload", UploadFile)
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("UploadFile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        return builder;
    }

    /// <summary>
    /// Handles the upload of a file to an AWS S3 storage.
    /// </summary>
    /// <param name="request">The file upload request containing the file and metadata.</param>
    /// <param name="awsS3Service">The service used for interacting with AWS S3.</param>
    /// <returns>An HTTP result containing the key code of the uploaded file if successful.</returns>
    private static async Task<IResult> UploadFile(
        [AsParameters] UploadFileRequest request,
        IAwsS3Service awsS3Service
    )
    {
        var keyCode = await awsS3Service.UploadFileAsync(request);

        return Results.Ok(keyCode);
    }
}
