namespace SharedKernel.FirebaseNotification.Models;

public sealed record FirebaseNotificationDispatchResult(
    string AppName,
    string? ProjectId,
    bool DryRun,
    int RequestedTokenCount,
    int UniqueTokenCount,
    int SuccessCount,
    int FailureCount,
    IReadOnlyList<FirebaseNotificationTokenResult> TokenResults
);
