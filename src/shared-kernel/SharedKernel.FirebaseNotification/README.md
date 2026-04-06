# SharedKernel.FirebaseNotification

Plugin `shared-kernel` để gửi mobile push notification qua Firebase Cloud Messaging (FCM).

## Registration

```csharp
builder.Services.AddFirebaseNotification(builder.Configuration);
```

## Configuration

```json
{
  "FirebaseNotification": {
    "AppName": "shared-kernel-firebase-notification",
    "ProjectId": "your-firebase-project-id",
    "CredentialPath": "secrets/firebase-service-account.json",
    "CredentialJson": "",
    "UseApplicationDefaultCredentials": true,
    "DefaultDryRun": false
  }
}
```

- `CredentialPath`: đường dẫn đến Firebase service account JSON.
- `CredentialJson`: nội dung service account JSON nếu muốn inject từ secret store.
- `UseApplicationDefaultCredentials`: dùng Application Default Credentials khi không cấu hình file/json.
- `DefaultDryRun`: mặc định chỉ validate payload với FCM, không gửi thật.

## Usage

```csharp
public sealed class PromotionPublisher(
    IFirebaseNotificationService firebaseNotificationService
)
{
    public Task<FirebaseNotificationDispatchResult> PublishAsync(
        IReadOnlyCollection<string> deviceTokens,
        CancellationToken cancellationToken
    )
    {
        return firebaseNotificationService.SendAsync(
            new FirebaseNotificationPayload(
                deviceTokens,
                "Promotion",
                "Flash sale starts now.",
                new Dictionary<string, string?>
                {
                    ["screen"] = "promotion",
                    ["campaign"] = "flash-sale"
                }
            ),
            cancellationToken
        );
    }
}
```

## Notes

- Mỗi request multicast hỗ trợ tối đa `500` device tokens.
- Plugin dùng `FirebaseAdmin` SDK và `SendEachForMulticastAsync` để trả về trạng thái theo từng device token.
- Để gửi thật, backend cần credential hợp lệ và mobile app phải cung cấp FCM registration token hợp lệ.
