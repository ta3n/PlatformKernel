# Hướng Dẫn Tích Hợp Firebase Cloud Messaging (FCM)

> **Plugin**: `SharedKernel.FirebaseNotification`
> **Mục đích**: Gửi push notification từ backend service đến mobile apps (iOS & Android)
> **SDK**: FirebaseAdmin 3.5.0

---

## 📋 Mục Lục

1. [Tổng Quan Kiến Trúc](#1-tổng-quan-kiến-trúc)
2. [Tạo Tài Khoản Firebase](#2-tạo-tài-khoản-firebase)
3. [Cấu Hình Firebase Project](#3-cấu-hình-firebase-project)
4. [Tạo Service Account Credentials](#4-tạo-service-account-credentials)
5. [Cấu Hình Backend Service](#5-cấu-hình-backend-service)
6. [Tích Hợp Vào Code](#6-tích-hợp-vào-code)
7. [Testing & Validation](#7-testing--validation)
8. [Troubleshooting](#8-troubleshooting)
9. [Best Practices](#9-best-practices)

---

## 1. Tổng Quan Kiến Trúc

### Flow Hoạt Động

```
┌─────────────────┐         ┌──────────────────┐         ┌─────────────────┐
│  Mobile App     │         │  Backend Service │         │  Firebase FCM   │
│  (iOS/Android)  │         │  (.NET 8)        │         │  Cloud          │
└─────────────────┘         └──────────────────┘         └─────────────────┘
        │                            │                            │
        │  1. Register FCM Token     │                            │
        ├───────────────────────────>│                            │
        │                            │                            │
        │                            │  2. Send Notification      │
        │                            ├───────────────────────────>│
        │                            │                            │
        │  3. Push Notification      │                            │
        │<──────────────────────────────────────────────────────┤
        │                            │                            │
```

### Thành Phần Chính

- **FirebaseAdmin SDK**: Thư viện chính thức từ Google để gửi FCM messages
- **Service Account**: Credential để backend authenticate với Firebase
- **Device Token**: Unique token của mỗi device app (do mobile app cung cấp)
- **Multicast Messaging**: Gửi 1 message đến nhiều devices (max 500 tokens/request)

---

## 2. Tạo Tài Khoản Firebase

### Bước 1: Truy cập Firebase Console

1. Mở trình duyệt và truy cập: **https://console.firebase.google.com/**
2. Đăng nhập bằng **Google Account** (tài khoản Gmail cá nhân hoặc Google Workspace)

### Bước 2: Tạo Firebase Project

1. Click nút **"Add project"** hoặc **"Create a project"**
2. Nhập thông tin project:

   ```
   Project name: platform-kernel-notifications
   ```

3. **Google Analytics** (Optional):
   - Tùy chọn: Có thể **tắt** nếu không cần analytics
   - Khuyến nghị: **Bật** để tracking notification performance
   - Nếu bật, chọn hoặc tạo **Analytics Account**

4. Click **"Create project"** và chờ ~30 giây

### Bước 3: Verify Project Creation

Sau khi tạo xong, bạn sẽ thấy:
- **Project Overview** dashboard
- **Project ID** (dạng: `platform-kernel-notifications-a1b2c`)
- **Project Number** (dạng: `123456789012`)

> **⚠️ Lưu ý**: **Project ID** sẽ được sử dụng trong configuration.

---

## 3. Cấu Hình Firebase Project

### Bước 1: Thêm Mobile App vào Project

#### Cho iOS App

1. Trong **Project Overview**, click **"Add app"** → chọn **iOS**
2. Nhập thông tin:
   ```
   iOS bundle ID: com.platformkernel.mobile
   App nickname: PlatformKernel iOS (optional)
   App Store ID: (có thể bỏ qua nếu chưa publish)
   ```
3. Download **`GoogleService-Info.plist`** file
4. Click **"Next"** → **"Continue to console"**

#### Cho Android App

1. Trong **Project Overview**, click **"Add app"** → chọn **Android**
2. Nhập thông tin:
   ```
   Android package name: com.platformkernel.mobile
   App nickname: PlatformKernel Android (optional)
   Debug signing certificate SHA-1: (optional, dùng cho testing)
   ```
3. Download **`google-services.json`** file
4. Click **"Next"** → **"Continue to console"**

### Bước 2: Enable Cloud Messaging API

1. Vào **Project Settings** (⚙️ icon) → **Cloud Messaging** tab
2. Xem và lưu lại:
   - **Server key** (Legacy, không cần cho .NET SDK)
   - **Sender ID**

> **✅ Quan trọng**: Từ Firebase SDK v9+, không cần server key nữa, chỉ cần Service Account.

### Bước 3: Configure Cloud Messaging Settings (Optional)

1. Vào **Cloud Messaging** tab
2. Cấu hình **APNs** (cho iOS):
   - Upload **APNs Authentication Key** (.p8 file)
   - Hoặc upload **APNs Certificate** (.p12 file)
3. Cấu hình **Android**: Không cần thêm config

---

## 4. Tạo Service Account Credentials

### Phương Án 1: Download Service Account JSON File (Khuyến Nghị)

#### Bước 1: Tạo Service Account

1. Vào **Project Settings** (⚙️ icon) → **Service accounts** tab
2. Click **"Generate new private key"**
3. Popup hiện lên, click **"Generate key"**
4. File JSON sẽ tự động download về máy

#### Bước 2: Lưu Trữ Secure

```bash
# Tạo thư mục secrets trong project (đã có trong .gitignore)
mkdir -p secrets

# Copy file vào thư mục secrets
mv ~/Downloads/platform-kernel-notifications-*.json secrets/firebase-service-account.json

# Set permission chỉ owner read
chmod 600 secrets/firebase-service-account.json
```

#### Cấu Trúc File JSON

```json
{
  "type": "service_account",
  "project_id": "platform-kernel-notifications-a1b2c",
  "private_key_id": "abcdef1234567890",
  "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBA...\n-----END PRIVATE KEY-----\n",
  "client_email": "firebase-adminsdk-xxxxx@platform-kernel-notifications-a1b2c.iam.gserviceaccount.com",
  "client_id": "123456789012345678901",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-xxxxx%40platform-kernel-notifications-a1b2c.iam.gserviceaccount.com"
}
```

### Phương Án 2: Sử dụng Environment Variables (Production)

Với Kubernetes/Docker, inject JSON content vào environment variable:

```bash
export FIREBASE_CREDENTIAL_JSON=$(cat secrets/firebase-service-account.json)
```

### Phương Án 3: Sử dụng Google Cloud Application Default Credentials

Khi deploy lên **Google Cloud** (GKE, Cloud Run, App Engine):

```bash
# Service tự động authentication thông qua Workload Identity
# Không cần credential file
```

---

## 5. Cấu Hình Backend Service

### Cách 1: Sử dụng Credential File Path (Local Development)

**appsettings.Development.json**

```json
{
  "FirebaseNotification": {
    "AppName": "platform-kernel-backend",
    "ProjectId": "platform-kernel-notifications-a1b2c",
    "CredentialPath": "secrets/firebase-service-account.json",
    "UseApplicationDefaultCredentials": false,
    "DefaultDryRun": false
  }
}
```

### Cách 2: Sử dụng Credential JSON String (Docker/Kubernetes)

**appsettings.json**

```json
{
  "FirebaseNotification": {
    "AppName": "platform-kernel-backend",
    "ProjectId": "platform-kernel-notifications-a1b2c",
    "CredentialJson": "",
    "UseApplicationDefaultCredentials": false,
    "DefaultDryRun": false
  }
}
```

**docker-compose.yml**

```yaml
services:
  backend:
    environment:
      - FirebaseNotification__CredentialJson=${FIREBASE_CREDENTIAL_JSON}
```

### Cách 3: Sử dụng Application Default Credentials (Google Cloud)

**appsettings.Production.json**

```json
{
  "FirebaseNotification": {
    "AppName": "platform-kernel-backend",
    "ProjectId": "platform-kernel-notifications-a1b2c",
    "UseApplicationDefaultCredentials": true,
    "DefaultDryRun": false
  }
}
```

### Giải Thích Các Options

| Option | Type | Description |
|--------|------|-------------|
| `AppName` | `string` | Tên Firebase app instance (mặc định: `shared-kernel-firebase-notification`) |
| `ProjectId` | `string?` | Firebase project ID (tự động đọc từ credential nếu không cấu hình) |
| `CredentialPath` | `string?` | **Đường dẫn** đến service account JSON file |
| `CredentialJson` | `string?` | **Nội dung** của service account JSON (inject từ secret) |
| `UseApplicationDefaultCredentials` | `bool` | Sử dụng Google Cloud workload identity (chỉ dùng khi deploy lên GCP) |
| `DefaultDryRun` | `bool` | `true` = chỉ validate, không gửi thật; `false` = gửi thật |

### Validation Rules

Plugin sẽ **validate** khi start:

1. ❌ **Không được** cấu hình đồng thời `CredentialPath` VÀ `CredentialJson`
2. ✅ **Phải có ít nhất 1** trong 3: `CredentialPath`, `CredentialJson`, hoặc `UseApplicationDefaultCredentials=true`
3. ✅ `AppName` **bắt buộc**

---

## 6. Tích Hợp Vào Code

### Bước 1: Register Service trong DI Container

**Program.cs**

```csharp
using SharedKernel.FirebaseNotification;

var builder = WebApplication.CreateBuilder(args);

// Register Firebase Notification
builder.Services.AddFirebaseNotification(builder.Configuration);

var app = builder.Build();
await app.RunAsync();
```

### Bước 2: Inject và Sử Dụng Service

#### Ví Dụ 1: Minimal API Endpoint

```csharp
using SharedKernel.FirebaseNotification.Abstractions;
using SharedKernel.FirebaseNotification.Models;

app.MapPost("/api/promotions/notify", async (
    IFirebaseNotificationService firebaseService,
    CancellationToken cancellationToken
) =>
{
    var deviceTokens = new[]
    {
        "fGxY7k8dQ9...", // Token từ iOS app
        "eHwZ3m2nR5...", // Token từ Android app
    };

    var payload = new FirebaseNotificationPayload(
        DeviceTokens: deviceTokens,
        Title: "Flash Sale 🔥",
        Body: "Giảm giá 50% cho tất cả sản phẩm trong 24h!",
        Data: new Dictionary<string, string?>
        {
            ["screen"] = "promotion",
            ["campaign_id"] = "flash-sale-2026"
        },
        DryRun: false // Gửi thật
    );

    var result = await firebaseService.SendAsync(payload, cancellationToken);

    return Results.Ok(new
    {
        totalSent = result.UniqueTokenCount,
        successCount = result.SuccessCount,
        failureCount = result.FailureCount,
        details = result.TokenResults
    });
});
```

#### Ví Dụ 2: Service Class

```csharp
namespace PlatformKernel.NotificationService.Application;

public sealed class OrderNotificationService(
    IFirebaseNotificationService firebaseService,
    ILogger<OrderNotificationService> logger
)
{
    public async Task NotifyOrderShippedAsync(
        string orderId,
        IReadOnlyCollection<string> deviceTokens,
        CancellationToken cancellationToken = default
    )
    {
        if (deviceTokens.Count == 0)
        {
            logger.LogWarning(
                "No device tokens found for order {OrderId}. Skip notification.",
                orderId
            );
            return;
        }

        var payload = new FirebaseNotificationPayload(
            DeviceTokens: deviceTokens,
            Title: "Đơn hàng đã được giao",
            Body: $"Đơn hàng #{orderId} đang trên đường giao đến bạn.",
            Data: new Dictionary<string, string?>
            {
                ["type"] = "order_shipped",
                ["order_id"] = orderId,
                ["screen"] = "order_detail"
            }
        );

        var result = await firebaseService.SendAsync(payload, cancellationToken);

        logger.LogInformation(
            "Sent order shipped notification for {OrderId}. Success: {Success}/{Total}",
            orderId,
            result.SuccessCount,
            result.UniqueTokenCount
        );
    }
}
```

#### Ví Dụ 3: Background Job với Hangfire

```csharp
using Hangfire;

public sealed class NotificationJob(
    IFirebaseNotificationService firebaseService
)
{
    [AutomaticRetry(Attempts = 3)]
    public async Task SendDailyReminder(
        IReadOnlyCollection<string> deviceTokens
    )
    {
        var payload = new FirebaseNotificationPayload(
            DeviceTokens: deviceTokens,
            Title: "Nhắc nhở hàng ngày ⏰",
            Body: "Bạn có 3 nhiệm vụ chưa hoàn thành hôm nay!",
            Data: new Dictionary<string, string?>
            {
                ["type"] = "daily_reminder",
                ["screen"] = "tasks"
            }
        );

        var result = await firebaseService.SendAsync(payload, CancellationToken.None);

        if (result.FailureCount > 0)
        {
            // Log failed tokens for retry
            var failedTokens = result.TokenResults
                .Where(r => !r.IsSuccess)
                .Select(r => r.DeviceToken)
                .ToList();

            throw new InvalidOperationException(
                $"Failed to send to {failedTokens.Count} devices."
            );
        }
    }
}
```

### Bước 3: Handle Response Results

```csharp
var result = await firebaseService.SendAsync(payload, cancellationToken);

// Check overall success
if (result.SuccessCount == result.UniqueTokenCount)
{
    // ✅ Tất cả đều thành công
}

// Process individual token results
foreach (var tokenResult in result.TokenResults)
{
    if (tokenResult.IsSuccess)
    {
        // ✅ Token này gửi thành công
        Console.WriteLine($"Sent to {tokenResult.DeviceToken}: {tokenResult.MessageId}");
    }
    else
    {
        // ❌ Token này thất bại
        Console.WriteLine($"Failed {tokenResult.DeviceToken}: {tokenResult.ErrorCode} - {tokenResult.ErrorMessage}");

        // Handle specific error codes
        switch (tokenResult.ErrorCode)
        {
            case "InvalidRegistration":
            case "NotRegistered":
                // Token không hợp lệ hoặc app đã uninstall → Xóa khỏi database
                await RemoveInvalidTokenAsync(tokenResult.DeviceToken);
                break;

            case "SenderIdMismatch":
                // Token từ project khác → Xóa khỏi database
                await RemoveInvalidTokenAsync(tokenResult.DeviceToken);
                break;

            case "QuotaExceeded":
                // Vượt quota FCM → Retry sau
                await RetryLaterAsync(tokenResult.DeviceToken);
                break;

            default:
                // Lỗi khác → Log để investigate
                _logger.LogError("FCM Error: {ErrorCode} - {ErrorMessage}",
                    tokenResult.ErrorCode, tokenResult.ErrorMessage);
                break;
        }
    }
}
```

---

## 7. Testing & Validation

### A. Test Local với DryRun Mode

**appsettings.Development.json**

```json
{
  "FirebaseNotification": {
    "DefaultDryRun": true
  }
}
```

Khi `DryRun = true`:
- ✅ Validate payload với FCM API
- ✅ Check device tokens format
- ❌ **Không gửi** notification thật đến devices

### B. Get Device Token từ Mobile App

#### iOS (Swift)

```swift
import FirebaseMessaging

Messaging.messaging().token { token, error in
    if let error = error {
        print("Error fetching FCM token: \(error)")
    } else if let token = token {
        print("FCM token: \(token)")
        // Send token to backend
        sendTokenToBackend(token)
    }
}
```

#### Android (Kotlin)

```kotlin
import com.google.firebase.messaging.FirebaseMessaging

FirebaseMessaging.getInstance().token.addOnCompleteListener { task ->
    if (task.isSuccessful) {
        val token = task.result
        println("FCM token: $token")
        // Send token to backend
        sendTokenToBackend(token)
    }
}
```

### C. Test với HTTP Client

**test-firebase.http**

```http
POST http://localhost:5107/api/firebase-notifications/send
Content-Type: application/json

{
  "title": "Test Notification",
  "message": "This is a test from backend",
  "deviceTokens": [
    "YOUR_REAL_FCM_TOKEN_HERE"
  ],
  "dryRun": false,
  "data": {
    "test": "true"
  }
}
```

### D. Verify Notification Delivered

1. **Mobile App**: Check notification tray
2. **Firebase Console**: **Cloud Messaging** → **Campaign analytics**
3. **Backend Logs**: Check success/failure count

---

## 8. Troubleshooting

### Lỗi: "Firebase credential file was not found"

**Nguyên nhân**: File credential không tồn tại ở đường dẫn cấu hình

**Giải pháp**:
```bash
# Check file exists
ls -la secrets/firebase-service-account.json

# Check appsettings path
cat appsettings.Development.json | grep CredentialPath
```

### Lỗi: "InvalidRegistration" hoặc "NotRegistered"

**Nguyên nhân**: Device token không hợp lệ hoặc app đã uninstall

**Giải pháp**:
- Xóa token khỏi database
- Yêu cầu mobile app refresh token

### Lỗi: "SenderIdMismatch"

**Nguyên nhân**: Token từ Firebase project khác

**Giải pháp**:
- Verify mobile app dùng đúng `google-services.json` / `GoogleService-Info.plist`
- Rebuild mobile app với config đúng

### Lỗi: "QuotaExceeded"

**Nguyên nhân**: Vượt quota FCM (free tier: 1 million messages/month)

**Giải pháp**:
- Upgrade Firebase plan
- Implement rate limiting
- Batch notifications

### Lỗi: "At least one device token is required"

**Nguyên nhân**: Payload không có token hoặc tất cả tokens bị trim/filter

**Giải pháp**:
```csharp
// Validate before sending
if (deviceTokens == null || !deviceTokens.Any())
{
    throw new InvalidOperationException("No device tokens provided");
}
```

### Mobile App Không Nhận Được Notification

**Checklist**:

1. ✅ Mobile app đã register FCM token?
2. ✅ Token được gửi đúng về backend?
3. ✅ Backend gửi notification success? (check logs)
4. ✅ iOS: APNs certificate đã upload vào Firebase?
5. ✅ Android: `google-services.json` đã add vào project?
6. ✅ Mobile app đang chạy foreground/background?
7. ✅ Mobile OS notification permission enabled?

---

## 9. Best Practices

### 1. Token Management

```csharp
// ✅ DO: Store tokens in database with user mapping
public sealed class UserDeviceToken
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public string DeviceToken { get; init; } = string.Empty;
    public string Platform { get; init; } = string.Empty; // "iOS" | "Android"
    public DateTime RegisteredAt { get; init; }
    public DateTime? LastUsedAt { get; init; }
    public bool IsActive { get; init; } = true;
}

// ❌ DON'T: Hard-code tokens in code
```

### 2. Batch Processing

```csharp
// ✅ DO: Batch tokens in chunks of 500
public async Task SendToAllUsersAsync(
    string title,
    string body,
    CancellationToken cancellationToken
)
{
    var allTokens = await GetAllActiveTokensAsync(cancellationToken);

    var chunks = allTokens
        .Chunk(500) // Max 500 tokens per request
        .ToList();

    foreach (var chunk in chunks)
    {
        var payload = new FirebaseNotificationPayload(
            chunk,
            title,
            body
        );

        await firebaseService.SendAsync(payload, cancellationToken);
    }
}
```

### 3. Error Handling & Retry

```csharp
// ✅ DO: Handle failures and cleanup invalid tokens
public async Task SendWithCleanupAsync(
    FirebaseNotificationPayload payload,
    CancellationToken cancellationToken
)
{
    var result = await firebaseService.SendAsync(payload, cancellationToken);

    var invalidTokens = result.TokenResults
        .Where(r => !r.IsSuccess && IsInvalidTokenError(r.ErrorCode))
        .Select(r => r.DeviceToken)
        .ToList();

    if (invalidTokens.Count > 0)
    {
        await RemoveInvalidTokensAsync(invalidTokens, cancellationToken);
    }
}

private static bool IsInvalidTokenError(string? errorCode)
{
    return errorCode is "InvalidRegistration" or "NotRegistered" or "SenderIdMismatch";
}
```

### 4. Logging & Monitoring

```csharp
// ✅ DO: Log với structured logging
logger.LogInformation(
    "FCM notification sent. Success: {SuccessCount}/{TotalCount}, Failed: {FailureCount}",
    result.SuccessCount,
    result.UniqueTokenCount,
    result.FailureCount
);

// Setup alerts khi failure rate > 10%
if (result.FailureCount > 0)
{
    var failureRate = (double)result.FailureCount / result.UniqueTokenCount;
    if (failureRate > 0.1)
    {
        logger.LogWarning(
            "High FCM failure rate: {FailureRate:P} ({FailureCount}/{TotalCount})",
            failureRate,
            result.FailureCount,
            result.UniqueTokenCount
        );
    }
}
```

### 5. Security

```bash
# ✅ DO: Add secrets/ to .gitignore
echo "secrets/" >> .gitignore

# ✅ DO: Encrypt credentials trong production
# Sử dụng Azure Key Vault, AWS Secrets Manager, hoặc Kubernetes Secrets

# ❌ DON'T: Commit credential files vào Git
```

### 6. Testing Strategy

```csharp
// ✅ DO: Sử dụng DryRun cho automated tests
[Fact]
public async Task SendNotification_WithValidPayload_ShouldValidateSuccessfully()
{
    var payload = new FirebaseNotificationPayload(
        DeviceTokens: ["test-token-1", "test-token-2"],
        Title: "Test",
        Body: "Test message",
        DryRun: true // Chỉ validate, không gửi thật
    );

    var result = await _firebaseService.SendAsync(payload);

    Assert.Equal(2, result.UniqueTokenCount);
}
```

---

## 📚 Tài Liệu Tham Khảo

- [Firebase Console](https://console.firebase.google.com/)
- [Firebase Admin .NET SDK](https://firebase.google.com/docs/admin/setup)
- [FCM Architecture](https://firebase.google.com/docs/cloud-messaging/fcm-architecture)
- [APNs Integration](https://firebase.google.com/docs/cloud-messaging/ios/certs)
- [Error Codes Reference](https://firebase.google.com/docs/cloud-messaging/send-message#admin_sdk_error_reference)

---

## ✅ Checklist Tích Hợp

- [ ] Tạo Firebase project
- [ ] Thêm iOS và Android apps vào project
- [ ] Upload APNs certificate (cho iOS)
- [ ] Download service account JSON
- [ ] Lưu credential file vào `secrets/` folder
- [ ] Cấu hình `appsettings.json`
- [ ] Register `AddFirebaseNotification()` trong `Program.cs`
- [ ] Implement notification logic
- [ ] Test với DryRun mode
- [ ] Get device token từ mobile app
- [ ] Test với real device
- [ ] Setup logging & monitoring
- [ ] Deploy lên production
- [ ] Verify notification delivered

---

**Phiên bản**: 1.0
**Cập nhật**: April 2026
**Tác giả**: Platform Kernel Team
