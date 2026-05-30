using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Options;
using SharedKernel.FirebaseNotification.Abstractions;
using SharedKernel.FirebaseNotification.Models;
using SharedKernel.FirebaseNotification.Options;
using SharedKernel.FirebaseNotification.Test.Service.Contracts;

namespace SharedKernel.FirebaseNotification.Test.Service.Api;

public static class FirebaseNotificationEndpoints
{
    private const int MaxTokensPerRequest = 500;

    public static IEndpointRouteBuilder MapFirebaseNotificationEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        endpoints.MapGet(
            "/",
            (
                IOptions<FirebaseNotificationOptions> options
            ) => Results.Ok(
                new
                {
                    service = "SharedKernel.FirebaseNotification.Test.Service",
                    plugin = "SharedKernel.FirebaseNotification",
                    endpoint = "/api/firebase-notifications/send",
                    defaultDryRun = options.Value.DefaultDryRun,
                    credentialSource = ResolveCredentialSource(options.Value),
                    appName = options.Value.AppName,
                    projectId = options.Value.ProjectId
                }
            )
        );

        var group = endpoints.MapGroup("/api/firebase-notifications")
            .WithTags("FirebaseNotification");

        group.MapPost("/send", SendAsync);

        return endpoints;
    }

    private static async Task<IResult> SendAsync(
        SendFirebaseNotificationRequest request,
        IFirebaseNotificationService firebaseNotificationService,
        CancellationToken cancellationToken
    )
    {
        var validationProblem = ValidateRequest(request);
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        var payload = new FirebaseNotificationPayload(
            request.DeviceTokens!
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .Select(token => token.Trim())
                .ToArray(),
            request.Title!.Trim(),
            request.Message!.Trim(),
            request.Data,
            request.DryRun
        );

        try
        {
            var result = await firebaseNotificationService.SendAsync(payload, cancellationToken)
                .ConfigureAwait(false);

            return TypedResults.Ok(result);
        }
        catch (ArgumentException exception)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]> { ["request"] = [exception.Message] }
            );
        }
        catch (FirebaseMessagingException exception)
        {
            return TypedResults.Problem(
                title: "Firebase delivery failed.",
                detail: exception.Message,
                statusCode: StatusCodes.Status502BadGateway
            );
        }
        catch (InvalidOperationException exception)
        {
            return TypedResults.Problem(
                title: "Firebase notification is not configured correctly.",
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    private static IResult? ValidateRequest(
        SendFirebaseNotificationRequest request
    )
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors[nameof(request.Title)] = ["Title is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            errors[nameof(request.Message)] = ["Message is required."];
        }

        var tokenCount = request.DeviceTokens?
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .Select(token => token.Trim())
                .Distinct(StringComparer.Ordinal)
                .Count()
            ?? 0;

        if (tokenCount == 0)
        {
            errors[nameof(request.DeviceTokens)] = ["At least one device token is required."];
        }
        else if (tokenCount > MaxTokensPerRequest)
        {
            errors[nameof(request.DeviceTokens)] =
                [$"Firebase multicast supports up to {MaxTokensPerRequest} device tokens per request."];
        }

        return errors.Count == 0 ? null : TypedResults.ValidationProblem(errors);
    }

    private static string ResolveCredentialSource(
        FirebaseNotificationOptions options
    )
    {
        if (!string.IsNullOrWhiteSpace(options.CredentialPath))
        {
            return "CredentialPath";
        }

        if (!string.IsNullOrWhiteSpace(options.CredentialJson))
        {
            return "CredentialJson";
        }

        return options.UseApplicationDefaultCredentials
            ? "ApplicationDefaultCredentials"
            : "NotConfigured";
    }
}
