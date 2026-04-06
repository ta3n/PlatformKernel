using SharedKernel.FirebaseNotification.Models;

namespace SharedKernel.FirebaseNotification.Abstractions;

public interface IFirebaseNotificationService
{
    Task<FirebaseNotificationDispatchResult> SendAsync(
        FirebaseNotificationPayload payload,
        CancellationToken cancellationToken = default
    );
}
