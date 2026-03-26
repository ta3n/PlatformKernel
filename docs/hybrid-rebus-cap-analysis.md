# Hybrid Solution: Rebus (Saga) + CAP (Outbox)

## 🎯 Concept

Kết hợp 2 frameworks để tận dụng điểm mạnh:
- **CAP**: Best transactional outbox với EF Core
- **Rebus**: Best saga state machine implementation

## 🏗️ Architecture

```
┌─────────────────────────────────────────────┐
│           Application Layer                  │
├─────────────────────────────────────────────┤
│  Domain Events  →  CAP Publisher (Outbox)   │
│                                              │
│  Saga Logic     →  Rebus Saga Manager       │
├─────────────────────────────────────────────┤
│           Message Broker                     │
│         (RabbitMQ / Kafka)                   │
└─────────────────────────────────────────────┘
```

## ✅ Feasibility: **CÓ THỂ IMPLEMENT**

### Approach 1: CAP cho Publishing, Rebus cho Consuming + Saga

```csharp
// 1. Domain events publish qua CAP (with outbox)
public class OrderService
{
    private readonly AppDbContext _db;
    private readonly ICapPublisher _cap;

    public async Task CreateOrder(Order order)
    {
        using var trans = _db.Database.BeginTransaction(_cap);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Publish qua CAP (guaranteed delivery via outbox)
        await _cap.PublishAsync("order.submitted", new OrderSubmitted
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            Amount = order.Amount
        });

        await trans.CommitAsync();
    }
}

// 2. Rebus Saga consume từ cùng broker
public class OrderSaga : Saga<OrderSagaData>,
    IAmInitiatedBy<OrderSubmitted>,
    IHandleMessages<InventoryReserved>,
    IHandleMessages<PaymentProcessed>
{
    private readonly ICapPublisher _cap; // Inject CAP để publish

    public OrderSaga(ICapPublisher cap)
    {
        _cap = cap;
    }

    protected override void CorrelateMessages(ICorrelationConfig<OrderSagaData> config)
    {
        config.Correlate<OrderSubmitted>(m => m.OrderId, d => d.OrderId);
        config.Correlate<InventoryReserved>(m => m.OrderId, d => d.OrderId);
        config.Correlate<PaymentProcessed>(m => m.OrderId, d => d.OrderId);
    }

    public async Task Handle(OrderSubmitted message)
    {
        Data.OrderId = message.OrderId;
        Data.State = "AwaitingInventory";

        // Publish command qua CAP (with outbox)
        await _cap.PublishAsync("reserve.inventory",
            new ReserveInventory { OrderId = message.OrderId });
    }

    public async Task Handle(InventoryReserved message)
    {
        Data.State = "AwaitingPayment";

        await _cap.PublishAsync("process.payment",
            new ProcessPayment
            {
                OrderId = Data.OrderId,
                Amount = Data.Amount
            });
    }

    public async Task Handle(PaymentProcessed message)
    {
        Data.State = "Completed";

        await _cap.PublishAsync("order.completed",
            new OrderCompleted { OrderId = Data.OrderId });

        MarkAsComplete();
    }
}
```

### Configuration

```csharp
public static class HybridMessagingExtensions
{
    public static IServiceCollection AddHybridMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var brokerUrl = configuration["MessageBroker:RabbitMQ:Url"];

        // 1. Setup CAP (Outbox + Publishing)
        services.AddCap(x =>
        {
            // Outbox with EF Core
            x.UseEntityFramework<AppDbContext>();

            // RabbitMQ transport
            x.UseRabbitMQ(o =>
            {
                o.HostName = brokerUrl;
                o.ExchangeName = "platform.events"; // Shared exchange
            });

            x.UseDashboard();
            x.FailedRetryCount = 3;
        });

        // 2. Setup Rebus (Saga + Consuming)
        services.AddRebus(configure => configure
            .Transport(t => t.UseRabbitMq(brokerUrl, "sagas-input"))

            // Saga persistence in PostgreSQL
            .Sagas(s => s.StoreInPostgres(
                configuration.GetConnectionString("Default"),
                "Sagas"))

            // Subscribe to events from CAP
            .Routing(r => r.TypeBased()
                .MapAssembly(typeof(OrderSubmitted).Assembly, "platform.events"))
        );

        // Register saga
        services.AutoRegisterHandlersFromAssemblyOf<OrderSaga>();

        return services;
    }
}
```

## 📊 Ưu điểm của Hybrid Approach

### ✅ Pros
1. **Best-of-both worlds**
   - CAP: Transactional outbox tốt nhất
   - Rebus: Saga implementation sạch đẹp

2. **Separation of Concerns**
   - CAP: Publishing layer (write side)
   - Rebus: Saga orchestration (workflow side)

3. **Flexibility**
   - Có thể dùng CAP subscribers cho simple consumers
   - Dùng Rebus saga cho complex workflows

4. **100% Free**
   - Cả 2 đều MIT license

5. **Reduced Migration Risk**
   - Migrate từng phần (outbox trước, saga sau)
   - Incremental adoption

## ⚠️ Nhược điểm & Challenges

### ❌ Cons

1. **Complexity tăng gấp đôi**
   - 2 frameworks cần maintain
   - 2 sets of configurations
   - 2 learning curves

2. **Message Format Coordination**
   - CAP và Rebus serialize khác nhau
   - Cần unified message contracts

3. **Database Tables**
   - CAP tables: `Cap.Published`, `Cap.Received`
   - Rebus tables: `Sagas`, `Timeouts`
   - Tổng ~6-8 tables

4. **Monitoring phức tạp**
   - CAP Dashboard riêng
   - Rebus không có built-in dashboard
   - Cần correlation tracking

5. **Potential Race Conditions**
   - CAP publish → Rebus consume
   - Timing issues nếu không careful

6. **Transaction Boundaries**
   - CAP transaction vs Rebus transaction
   - Cần careful coordination

## 🔧 Implementation Challenges & Solutions

### Challenge 1: Message Serialization

**Problem:** CAP dùng JSON, Rebus có thể dùng format khác

**Solution:**
```csharp
// Unified message contract
public interface IMessage
{
    Guid MessageId { get; }
    DateTime Timestamp { get; }
}

public record OrderSubmitted : IMessage
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
}

// CAP config
services.AddCap(x =>
{
    x.UseNewtonsoftJson(); // Explicit serializer
});

// Rebus config
services.AddRebus(c => c
    .Serialization(s => s.UseNewtonsoftJson())); // Same serializer
```

### Challenge 2: Topic/Queue Naming

**Problem:** CAP và Rebus có convention khác nhau

**Solution:**
```csharp
// Naming convention helper
public static class MessagingTopics
{
    // CAP topics
    public const string OrderSubmitted = "order.submitted";
    public const string InventoryReserved = "inventory.reserved";
    public const string PaymentProcessed = "payment.processed";

    // Rebus subscribes to same topics
}

// CAP
await _cap.PublishAsync(MessagingTopics.OrderSubmitted, message);

// Rebus routing
.Routing(r => r.AddTransientSubscription(MessagingTopics.OrderSubmitted))
```

### Challenge 3: Idempotency

**Problem:** Message có thể được process 2 lần (CAP consumer + Rebus saga)

**Solution:**
```csharp
// Option A: Chỉ dùng Rebus saga, disable CAP consumer cho saga messages
services.AddCap(x =>
{
    x.SubscriberFilter = (topic, group) =>
    {
        // Skip saga-related topics
        var sagaTopics = new[] { "order.submitted", "inventory.reserved" };
        return !sagaTopics.Contains(topic);
    };
});

// Option B: Idempotency check trong Rebus saga
public async Task Handle(OrderSubmitted message)
{
    if (IsAlreadyProcessed(message.MessageId))
        return;

    // Process...
}
```

### Challenge 4: Transaction Coordination

**Problem:** CAP transaction riêng, Rebus transaction riêng

**Solution:**
```csharp
// Publish trong Rebus saga via CAP outbox
public class OrderSaga : Saga<OrderSagaData>
{
    private readonly ICapPublisher _cap;
    private readonly AppDbContext _db;

    public async Task Handle(OrderSubmitted message)
    {
        // Update saga state
        Data.State = "AwaitingInventory";

        // Publish qua CAP với transaction
        using var trans = _db.Database.BeginTransaction(_cap);

        await _cap.PublishAsync("reserve.inventory",
            new ReserveInventory { OrderId = message.OrderId });

        await trans.CommitAsync();
    }
}
```

## 📈 So sánh với Alternatives

| Aspect | MassTransit 8.x | Rebus + CAP | Pure CAP | Pure Rebus |
|--------|----------------|-------------|----------|------------|
| **Saga** | ✅✅ Excellent | ✅ Good | 🟡 Custom | ✅ Good |
| **Outbox** | ✅ Good | ✅✅ Excellent (CAP) | ✅✅ Excellent | 🟡 Via extensions |
| **Batch Consumer** | ✅ | ✅ (Rebus) | ❌ | ✅ |
| **Complexity** | ⭐⭐ Simple | ⭐⭐⭐⭐ Complex | ⭐⭐ Simple | ⭐⭐⭐ Medium |
| **Free** | ✅ | ✅ | ✅ | ✅ |
| **Migration Effort** | Zero | 🔴 Very High | 🔴 High | 🔴 High |
| **Maintenance** | ⭐⭐ Low | ⭐⭐⭐⭐ High | ⭐⭐ Low | ⭐⭐⭐ Medium |

## 💰 Cost Analysis (3 years)

| Cost Item | MassTransit 8.x | Rebus + CAP | Pure CAP |
|-----------|----------------|-------------|----------|
| **License** | $0 | $0 | $0 |
| **Migration** | $0 | $25,000 | $15,000 |
| **Learning** | $0 | $8,000 | $3,000 |
| **Maintenance** | $15,000 | $25,000 | $10,000 |
| **Bug fixes** | $5,000 | $15,000 | $8,000 |
| **Total** | **$20,000** | **$73,000** | **$36,000** |

## 🎯 Verdict: NÊN hay KHÔNG?

### ❌ **KHÔNG NÊN sử dụng Rebus + CAP**

**Lý do:**

1. **Complexity không đáng giá**
   - Tăng 3.6x cost so với giữ MassTransit 8.x
   - 2x complexity so với pure CAP
   - Khó maintain, debug, monitor

2. **"Best of both" không đủ lợi**
   - Rebus saga chỉ tốt hơn CAP một chút
   - MassTransit saga vẫn tốt hơn cả 2
   - CAP outbox tốt nhưng không bù được complexity

3. **Integration challenges lớn**
   - Message format coordination
   - Transaction boundary issues
   - Race conditions potential
   - Monitoring nightmare

4. **Team overhead cao**
   - 2 frameworks = 2x learning curve
   - More bugs, more maintenance
   - Harder to onboard new devs

## 🏆 Alternative Recommendation

### If Saga is CRITICAL → **Giữ MassTransit 8.x**

Vì:
- Best saga implementation
- Zero migration cost
- Low complexity
- Team đã quen

### If Outbox is CRITICAL → **Pure CAP + Custom Saga**

Vì:
- Best outbox implementation
- Simpler than Rebus + CAP
- Lower cost
- Single framework to maintain

## 📋 Decision Matrix

```
Nếu bạn cần:
├── Saga Complex + Batch → Giữ MassTransit 8.x ✅
├── Outbox Best-in-class → Pure CAP (custom saga) 🟡
├── Balance Saga + Outbox → Giữ MassTransit 8.x ✅
└── Lowest Cost → Giữ MassTransit 8.x ✅

Hybrid Rebus + CAP chỉ hợp lý nếu:
├── Team có 2 experts (1 Rebus, 1 CAP) ✅
├── Budget unlimited 💰
├── Thích challenge 🤷
└── Prototype/Research project 🔬
```

## 🚫 Bottom Line

**Rebus + CAP = Over-engineering**

Công thức tốt hơn:
- **Production app:** Giữ MassTransit 8.x
- **Greenfield app (no Kafka):** Wolverine
- **Outbox-first app:** Pure CAP
- **Budget có:** NServiceBus

**Không bao giờ:** Rebus + CAP trong production

## 📚 References

- Rebus Saga: https://github.com/rebus-org/Rebus/wiki/Sagas
- CAP Outbox: https://cap.dotnetcore.xyz/user-guide/en/cap/messaging/
- Transaction Patterns: https://microservices.io/patterns/data/transactional-outbox.html

---

**Kết luận cuối cùng:** Stick với MassTransit 8.x! 🎯
