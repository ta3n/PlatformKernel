# SharedKernel.FirebaseNotification

Plugin `shared-kernel` để gửi mobile push notification qua Firebase Cloud Messaging (FCM).

## 🚀 Quick Start

Xem [Firebase Quick Start Guide](../../../docs/firebase-quick-start.md) để setup trong **15 phút**.

## 📚 Documentation

- **[Quick Start Guide](../../../docs/firebase-quick-start.md)** - Setup nhanh cho development (15 phút)
- **[Integration Guide](../../../docs/firebase-integration-guide.md)** - Hướng dẫn tích hợp đầy đủ (tất cả bước từ A-Z)
- **[Code Review](../../../docs/firebase-code-review.md)** - Review chi tiết kiến trúc và code quality

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

### Configuration Options

| Option                             | Type      | Required | Description                                      |
|------------------------------------|-----------|----------|--------------------------------------------------|
| `AppName`                          | `string`  | Yes      | Tên Firebase app instance                        |
| `ProjectId`                        | `string?` | No       | Firebase project ID (auto-detect từ credential)  |
| `CredentialPath`                   | `string?` | No*      | Đường dẫn đến service account JSON file          |
| `CredentialJson`                   | `string?` | No*      | Nội dung service account JSON (inject từ secret) |
| `UseApplicationDefaultCredentials` | `bool`    | No*      | Dùng Google Cloud workload identity              |
| `DefaultDryRun`                    | `bool`    | No       | `true` = validate only, `false` = gửi thật       |

**\* At least one credential source required**

### Credential Sources (Pick One)

```bash
# Option 1: File path (Local development)
"CredentialPath": "secrets/firebase-service-account.json"

# Option 2: JSON string (Docker/Kubernetes)
"CredentialJson": "{\"type\":\"service_account\",...}"

# Option 3: Application Default Credentials (Google Cloud)
"UseApplicationDefaultCredentials": true
```

## Usage

### Basic Example

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

### Handle Response

```csharp
var result = await firebaseService.SendAsync(payload, cancellationToken);

// Check overall success
Console.WriteLine($"Success: {result.SuccessCount}/{result.UniqueTokenCount}");

// Process individual token results
foreach (var tokenResult in result.TokenResults)
{
    if (tokenResult.IsSuccess)
    {
        Console.WriteLine($"✅ Sent to {tokenResult.DeviceToken}");
    }
    else
    {
        Console.WriteLine($"❌ Failed {tokenResult.DeviceToken}: {tokenResult.ErrorCode}");

        // Handle invalid tokens
        if (tokenResult.ErrorCode is "InvalidRegistration" or "NotRegistered")
        {
            await RemoveTokenFromDatabaseAsync(tokenResult.DeviceToken);
        }
    }
}
```

## Testing

### Local Testing với Test Service

```bash
cd src/shared-kernel/SharedKernel.FirebaseNotification.Test.Service
dotnet run

# Service starts at http://localhost:5107
```

Sử dụng file `.http` để test:

```http
POST http://localhost:5107/api/firebase-notifications/send
Content-Type: application/json

{
  "title": "Test Notification",
  "message": "Hello from backend!",
  "deviceTokens": ["YOUR_FCM_TOKEN"],
  "dryRun": true
}
```

### DryRun Mode

Set `"DefaultDryRun": true` trong appsettings để:

- ✅ Validate payload với FCM API
- ✅ Check device tokens format
- ❌ **Không** gửi notification thực tế đến devices

## Features

- ✅ **Multiple credential sources** - File, JSON string, hoặc Application Default Credentials
- ✅ **Multicast support** - Gửi đến 500 devices trong 1 request
- ✅ **Per-token results** - Chi tiết success/failure cho từng token
- ✅ **Token normalization** - Auto trim, deduplicate, filter empty tokens
- ✅ **Comprehensive validation** - Fail fast với clear error messages
- ✅ **DryRun mode** - Test mà không gửi thật
- ✅ **Structured logging** - Rich observability
- ✅ **Thread-safe singleton** - Efficient resource management

## Notes

- Mỗi request multicast hỗ trợ tối đa **500** device tokens.
- Plugin dùng `FirebaseAdmin` SDK và `SendEachForMulticastAsync` để trả về trạng thái theo từng device token.
- Để gửi thật, backend cần credential hợp lệ và mobile app phải cung cấp FCM registration token hợp lệ.
- Xem [Integration Guide](../../../docs/firebase-integration-guide.md) để biết cách setup Firebase project và get device
  tokens.

## Troubleshooting

### "Firebase credential file was not found"

```bash
# Verify file exists
ls -la secrets/firebase-service-account.json
```

### "InvalidRegistration" errors

Device token không hợp lệ hoặc app đã uninstall. Xóa token khỏi database.

### Mobile app không nhận notification

1. ✅ iOS: Upload APNs certificate vào Firebase Console
2. ✅ Android: Add `google-services.json` vào project
3. ✅ Mobile app request notification permission
4. ✅ Set `"DefaultDryRun": false` để gửi thật

Xem [Integration Guide](../../../docs/firebase-integration-guide.md) section 8 để biết thêm chi tiết.
