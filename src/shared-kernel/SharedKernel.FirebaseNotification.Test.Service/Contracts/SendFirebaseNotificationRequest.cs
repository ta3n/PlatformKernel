namespace SharedKernel.FirebaseNotification.Test.Service.Contracts;

public sealed class SendFirebaseNotificationRequest
{
    public string? Title { get; init; }

    public string? Message { get; init; }

    public IReadOnlyCollection<string>? DeviceTokens { get; init; }

    public IReadOnlyDictionary<string, string?>? Data { get; init; }

    public bool? DryRun { get; init; }
}
