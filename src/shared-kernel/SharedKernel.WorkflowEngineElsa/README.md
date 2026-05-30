# SharedKernel.WorkflowEngineElsa

`SharedKernel.WorkflowEngineElsa` là shared-kernel package để bọc Elsa Workflows theo hướng production:

- cấu hình runtime + management persistence cho Elsa v3 trên PostgreSQL
- cung cấp abstraction nhỏ để application layer start/resume workflow mà không leak Elsa types khắp codebase
- đăng ký reusable booking activities cho Redis hold layer nhằm chống overbooking

## Mục tiêu kiến trúc

- Workflow chỉ làm orchestration.
- Business logic và transactional write vẫn nằm trong Application/Domain layer.
- DB write + integration event phải đi qua CQRS command handler và MassTransit EF outbox.
- Redis được dùng như hold layer tốc độ cao trước khi commit DB.

## Cấu hình tối thiểu

```json
{
  "ConnectionStrings": {
    "WorkflowEngineElsa": "Host=localhost;Database=workflow_engine;Username=postgres;Password=postgres"
  },
  "WorkflowEngineElsa": {
    "UseEntityFrameworkPersistence": true,
    "RunMigrations": false,
    "EnableScheduling": true,
    "EnableHttpActivities": false,
    "ExposeWorkflowsApi": false
  }
}
```

## Đăng ký trong service

```csharp
using SharedKernel.WorkflowEngineElsa;

builder.Services.AddWorkflowEngineElsa(
    builder.Configuration,
    elsa => elsa.AddWorkflowsFrom<Program>()
);

var app = builder.Build();
app.UseWorkflowEngineElsa(builder.Configuration);
```

## Booking workflow reference

Production flow được khuyến nghị:

1. API/Application nhận `CreateBookingCommand`.
2. Workflow reserve Redis hold bằng `ReserveInventoryHoldActivity`.
3. Nếu hold fail, workflow dừng sớm và trả về `rejected`.
4. Nếu hold success, activity tiếp theo gọi CQRS command để:
  - insert booking row trạng thái `PendingPayment`
  - ghi outbox event `BookingPending`
  - commit transaction
5. Workflow chờ stimulus như `booking/payment-completed` hoặc `booking/payment-failed`.
6. Khi payment success:
  - command handler cập nhật DB sang `Confirmed`
  - publish integration event qua outbox
  - workflow gọi `ConfirmInventoryHoldActivity`
7. Khi payment fail/timeout/cancel:
  - command handler cập nhật DB sang `Cancelled` nếu cần
  - workflow gọi `ReleaseInventoryHoldActivity`

## Elsa + MassTransit + Outbox

Pattern khuyến nghị:

- Elsa quyết định bước tiếp theo.
- MassTransit chịu trách nhiệm transport và consumer topology.
- EF outbox đảm bảo DB commit và publish event là atomic.

Không nên publish business event trực tiếp từ workflow activity nếu event đó phải cùng transaction với aggregate write.
Thay vào đó:

- workflow activity gọi application service / MediatR command
- command handler thực hiện DB write
- command handler gọi `IPublishEndpoint`
- `SharedKernel.MassTransit.AddEntityFrameworkOutboxCustom<TDbContext>()` lo phần outbox

## Ghi chú production

- `RunMigrations` nên tắt ở production, dùng migration pipeline riêng.
- Redis hold là optimistic admission layer, không thay thế hoàn toàn DB constraints và reconciliation job.
- Correlation ID nên dùng `BookingId` hoặc `ReservationId` xuyên suốt workflow, DB row và event bus.
