namespace SharedKernel.FirebaseNotification.Models;

public sealed record FirebaseNotificationTokenResult(
    string DeviceToken,
    bool IsSuccess,
    string? MessageId,
    string? ErrorCode,
    string? ErrorMessage
);
