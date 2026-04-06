namespace SharedKernel.FirebaseNotification.Models;

public sealed record FirebaseNotificationPayload(
    IReadOnlyCollection<string> DeviceTokens,
    string Title,
    string Body,
    IReadOnlyDictionary<string, string?>? Data = null,
    bool? DryRun = null
);
