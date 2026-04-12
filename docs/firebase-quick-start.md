# Firebase Notification - Quick Start Guide

> **Mục tiêu**: Gửi notification từ backend lên mobile app trong **15 phút** ⏱️

## 🚀 Setup Nhanh (Development)

### Bước 1: Tạo Firebase Project (5 phút)

1. Truy cập **https://console.firebase.google.com/**
2. Click **"Add project"**
3. Nhập tên: `platform-kernel-dev`
4. Tắt Google Analytics → **Create project**

### Bước 2: Download Service Account (2 phút)

1. Trong Firebase Console → **⚙️ Project Settings** → **Service accounts**
2. Click **"Generate new private key"**
3. Download file JSON (~2KB)
4. Rename thành: `firebase-service-account.json`

### Bước 3: Lưu Credential (1 phút)

```bash
# Tại root của PlatformKernel project
mkdir -p secrets
mv ~/Downloads/firebase-service-account.json secrets/
chmod 600 secrets/firebase-service-account.json
```

### Bước 4: Cấu Hình Backend (2 phút)

**appsettings.Development.json**:

```json
{
  "FirebaseNotification": {
    "AppName": "platform-kernel-backend",
    "CredentialPath": "secrets/firebase-service-account.json",
    "DefaultDryRun": true
  }
}
```

### Bước 5: Register Service (1 phút)

**Program.cs**:

```csharp
using SharedKernel.FirebaseNotification;

builder.Services.AddFirebaseNotification(builder.Configuration);
```

### Bước 6: Test với Test Service (2 phút)

```bash
# Start test service
cd src/shared-kernel/SharedKernel.FirebaseNotification.Test.Service
dotnet run

# Service starts at http://localhost:5107
```

Mở **SharedKernel.FirebaseNotification.Test.Service.http**:

```http
POST http://localhost:5107/api/firebase-notifications/send
Content-Type: application/json

{
  "title": "Test",
  "message": "Hello from backend!",
  "deviceTokens": ["fake-token-123"],
  "dryRun": true
}
```

**Expected Response** (DryRun mode):

```json
{
  "appName": "shared-kernel-firebase-notification-test-service",
  "projectId": "platform-kernel-dev",
  "dryRun": true,
  "requestedTokenCount": 1,
  "uniqueTokenCount": 1,
  "successCount": 1,
  "failureCount": 0,
  "tokenResults": [
    {
      "deviceToken": "fake-token-123",
      "isSuccess": true,
      "messageId": "projects/platform-kernel-dev/messages/0:1234567890",
      "errorCode": null,
      "errorMessage": null
    }
  ]
}
```

✅ **Nếu response như trên** → Firebase đã hoạt động!

### Bước 7: Gửi Thật Đến Device (2 phút)

#### A. Get Device Token từ Mobile App

**iOS Test App** - Add vào `AppDelegate.swift`:

```swift
import FirebaseMessaging

func application(_ application: UIApplication, 
                 didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
    FirebaseApp.configure()
    
    Messaging.messaging().token { token, error in
        if let token = token {
            print("📱 FCM Token: \(token)")
            // Copy token này
        }
    }
    
    return true
}
```

**Android Test App** - Add vào `MainActivity.kt`:

```kotlin
FirebaseMessaging.getInstance().token.addOnCompleteListener { task ->
    if (task.isSuccessful) {
        val token = task.result
        println("📱 FCM Token: $token")
        // Copy token này
    }
}
```

#### B. Send Notification với Real Token

**appsettings.Development.json** - Tắt DryRun:

```json
{
  "FirebaseNotification": {
    "DefaultDryRun": false
  }
}
```

**Test request**:

```http
POST http://localhost:5107/api/firebase-notifications/send
Content-Type: application/json

{
  "title": "🎉 First Notification",
  "message": "Firebase works perfectly!",
  "deviceTokens": ["YOUR_REAL_FCM_TOKEN_HERE"],
  "dryRun": false,
  "data": {
    "screen": "home"
  }
}
```

📱 **Check mobile device** → Notification xuất hiện!

---

## 💻 Sử Dụng trong Code

### Ví Dụ 1: Minimal API

```csharp
app.MapPost("/api/notify", async (
    IFirebaseNotificationService firebase,
    CancellationToken ct
) =>
{
    var result = await firebase.SendAsync(
        new FirebaseNotificationPayload(
            DeviceTokens: ["token1", "token2"],
            Title: "Flash Sale",
            Body: "50% off today only!"
        ),
        ct
    );

    return Results.Ok(new
    {
        sent = result.SuccessCount,
        failed = result.FailureCount
    });
});
```

### Ví Dụ 2: Service Class

```csharp
public sealed class OrderService(IFirebaseNotificationService firebase)
{
    public async Task NotifyOrderShippedAsync(
        string orderId,
        IReadOnlyCollection<string> userTokens
    )
    {
        await firebase.SendAsync(
            new FirebaseNotificationPayload(
                userTokens,
                "Order Shipped 📦",
                $"Order {orderId} is on the way!",
                new Dictionary<string, string?>
                {
                    ["order_id"] = orderId,
                    ["screen"] = "order_detail"
                }
            )
        );
    }
}
```

---

## 🔧 Troubleshooting

### ❌ "Firebase credential file was not found"

```bash
# Check file tồn tại
ls -la secrets/firebase-service-account.json

# Check quyền đọc
cat secrets/firebase-service-account.json | jq .project_id
```

### ❌ "InvalidRegistration" error

**Nguyên nhân**: Device token không hợp lệ

**Fix**: 
- Verify mobile app đã call `FirebaseApp.configure()`
- Check token format (dài ~150-200 chars)

### ❌ Mobile app không nhận notification

**Checklist**:
1. ✅ iOS: Upload APNs certificate vào Firebase Console
2. ✅ Android: Add `google-services.json` vào app
3. ✅ Mobile app request notification permission
4. ✅ Backend `dryRun: false`

---

## 📚 Next Steps

- [ ] Đọc [firebase-integration-guide.md](./firebase-integration-guide.md) - Hướng dẫn đầy đủ
- [ ] Đọc [firebase-code-review.md](./firebase-code-review.md) - Code review chi tiết
- [ ] Setup APNs certificate cho iOS (production)
- [ ] Implement token management database
- [ ] Setup monitoring & alerts

---

**⏱️ Total time**: ~15 phút  
**Status**: ✅ Ready for development

