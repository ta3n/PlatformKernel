using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.FirebaseNotification.Abstractions;
using SharedKernel.FirebaseNotification.Models;
using SharedKernel.FirebaseNotification.Options;

namespace SharedKernel.FirebaseNotification.Services;

internal sealed class FirebaseNotificationService(
    FirebaseAppProvider firebaseAppProvider,
    IOptions<FirebaseNotificationOptions> options,
    ILogger<FirebaseNotificationService> logger
) : IFirebaseNotificationService
{
    private const int MaxTokensPerRequest = 500;

    public async Task<FirebaseNotificationDispatchResult> SendAsync(
        FirebaseNotificationPayload payload,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(payload.DeviceTokens);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload.Title);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload.Body);

        var normalizedTokens = NormalizeTokens(payload.DeviceTokens);
        if (normalizedTokens.Count == 0)
        {
            throw new ArgumentException("At least one device token is required.", nameof(payload));
        }

        if (normalizedTokens.Count > MaxTokensPerRequest)
        {
            throw new ArgumentException(
                $"Firebase multicast supports up to {MaxTokensPerRequest} device tokens per request.",
                nameof(payload)
            );
        }

        var dryRun = payload.DryRun ?? options.Value.DefaultDryRun;
        var firebaseApp = firebaseAppProvider.GetApp();
        var messaging = FirebaseMessaging.GetMessaging(firebaseApp);
        var multicastMessage = new MulticastMessage
        {
            Tokens = normalizedTokens,
            Notification = new Notification
            {
                Title = payload.Title.Trim(),
                Body = payload.Body.Trim()
            },
            Data = NormalizeData(payload.Data)
        };

        logger.LogInformation(
            "Sending Firebase notification to {TokenCount} device(s). DryRun: {DryRun}.",
            normalizedTokens.Count,
            dryRun
        );

        var batchResponse = await messaging.SendEachForMulticastAsync(
                multicastMessage,
                dryRun,
                cancellationToken
            )
            .ConfigureAwait(false);

        var tokenResults = batchResponse.Responses
            .Select(
                (response, index) =>
                {
                    var exception = response.Exception;
                    return new FirebaseNotificationTokenResult(
                        normalizedTokens[index],
                        response.IsSuccess,
                        response.MessageId,
                        exception?.MessagingErrorCode.ToString(),
                        exception?.Message
                    );
                }
            )
            .ToArray();

        return new FirebaseNotificationDispatchResult(
            firebaseApp.Name,
            firebaseAppProvider.GetProjectId(),
            dryRun,
            payload.DeviceTokens.Count,
            normalizedTokens.Count,
            batchResponse.SuccessCount,
            batchResponse.FailureCount,
            tokenResults
        );
    }

    private static IReadOnlyList<string> NormalizeTokens(
        IEnumerable<string> deviceTokens
    )
    {
        var tokens = new List<string>();
        var uniqueTokens = new HashSet<string>(StringComparer.Ordinal);

        foreach (var deviceToken in deviceTokens)
        {
            if (string.IsNullOrWhiteSpace(deviceToken))
            {
                continue;
            }

            var trimmedToken = deviceToken.Trim();
            if (uniqueTokens.Add(trimmedToken))
            {
                tokens.Add(trimmedToken);
            }
        }

        return tokens;
    }

    private static IReadOnlyDictionary<string, string>? NormalizeData(
        IReadOnlyDictionary<string, string?>? data
    )
    {
        if (data is null || data.Count == 0)
        {
            return null;
        }

        var normalized = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var pair in data)
        {
            if (string.IsNullOrWhiteSpace(pair.Key))
            {
                continue;
            }

            normalized[pair.Key.Trim()] = pair.Value?.Trim() ?? string.Empty;
        }

        return normalized.Count == 0 ? null : normalized;
    }
}
