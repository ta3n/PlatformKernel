# Migration Plan: MassTransit → CAP

## Overview
Kế hoạch migration từ MassTransit 8.x sang CAP framework để tránh licensing costs của MassTransit 9.x.

## Phase 1: Setup CAP Infrastructure (Week 1)

### 1.1 Install CAP packages
```bash
dotnet add package DotNetCore.CAP
dotnet add package DotNetCore.CAP.PostgreSql
dotnet add package DotNetCore.CAP.RabbitMQ
# hoặc
dotnet add package DotNetCore.CAP.Kafka
dotnet add package DotNetCore.CAP.Dashboard
```

### 1.2 Configure CAP
```csharp
// SharedKernel.CAP/Extensions.cs
public static class Extensions
{
    public static IServiceCollection AddCapCustom(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mqOptions = configuration.GetSection("MessageQueueSettings")
            .Get<MessageQueueOptions>();

        services.AddCap(x =>
        {
            // PostgreSQL for outbox
            x.UseEntityFramework<YourDbContext>();

            // Transport
            if (mqOptions.QueueType == "RabbitMq")
            {
                x.UseRabbitMQ(o =>
                {
                    o.HostName = mqOptions.RabbitMqOptions.Host;
                    o.UserName = mqOptions.RabbitMqOptions.Username;
                    o.Password = mqOptions.RabbitMqOptions.Password;
                    o.Port = mqOptions.RabbitMqOptions.Port;
                });
            }
            else if (mqOptions.QueueType == "Kafka")
            {
                x.UseKafka(o =>
                {
                    o.Servers = mqOptions.KafkaOptions.Host;
                });
            }

            // Dashboard
            x.UseDashboard();

            // Policies
            x.FailedRetryCount = 3;
            x.FailedRetryInterval = 60;
            x.FailedThresholdCallback = (type, name, content) =>
            {
                // Log failed message
            };
        });

        return services;
    }
}
```

### 1.3 Database Migration
```csharp
// Add CAP tables to your DbContext
// CAP tự động tạo tables khi startup
// Tables: Cap.Published, Cap.Received
```

## Phase 2: Convert Consumers (Week 1-2)

### MassTransit Consumer
```csharp
// Before (MassTransit)
public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
{
    private readonly ILogger<OrderSubmittedConsumer> _logger;

    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        var message = context.Message;
        _logger.LogInformation("Order {OrderId} submitted", message.OrderId);

        // Process...

        await context.Publish(new InventoryReserved
        {
            OrderId = message.OrderId
        });
    }
}
```

### CAP Subscriber
```csharp
// After (CAP)
public class OrderSubmittedHandler : ICapSubscribe
{
    private readonly ILogger<OrderSubmittedHandler> _logger;
    private readonly ICapPublisher _publisher;

    [CapSubscribe("order.submitted")]
    public async Task Handle(OrderSubmitted message)
    {
        _logger.LogInformation("Order {OrderId} submitted", message.OrderId);

        // Process...

        await _publisher.PublishAsync("inventory.reserved",
            new InventoryReserved { OrderId = message.OrderId });
    }
}
```

## Phase 3: Convert Publishers (Week 2)

### MassTransit Publisher
```csharp
// Before (MassTransit)
public class OrderService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public async Task SubmitOrder(Order order)
    {
        await _publishEndpoint.Publish(new OrderSubmitted
        {
            OrderId = order.Id
        });
    }
}
```

### CAP Publisher
```csharp
// After (CAP)
public class OrderService
{
    private readonly ICapPublisher _publisher;
    private readonly YourDbContext _dbContext;

    public async Task SubmitOrder(Order order)
    {
        // CAP automatically wraps in transaction with DbContext
        using var trans = _dbContext.Database.BeginTransaction(_publisher);

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        await _publisher.PublishAsync("order.submitted",
            new OrderSubmitted { OrderId = order.Id });

        await trans.CommitAsync();
    }
}
```

## Phase 4: Handle Saga Pattern (Week 3)

CAP không có built-in Saga, cần implement State Machine pattern thủ công:

```csharp
// Order Saga State
public class OrderSagaState : IEntityBase
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string CurrentState { get; set; } = "Started";
    public bool InventoryReserved { get; set; }
    public bool PaymentProcessed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

// Saga Coordinator
public class OrderSagaCoordinator : ICapSubscribe
{
    private readonly YourDbContext _dbContext;
    private readonly ICapPublisher _publisher;

    // Step 1: Start Saga
    [CapSubscribe("order.submitted")]
    public async Task OnOrderSubmitted(OrderSubmitted message)
    {
        var saga = new OrderSagaState
        {
            Id = Guid.NewGuid(),
            OrderId = message.OrderId,
            CurrentState = "InventoryReservation",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.OrderSagas.Add(saga);
        await _dbContext.SaveChangesAsync();

        await _publisher.PublishAsync("reserve.inventory",
            new ReserveInventory { OrderId = message.OrderId });
    }

    // Step 2: Inventory Reserved
    [CapSubscribe("inventory.reserved")]
    public async Task OnInventoryReserved(InventoryReserved message)
    {
        var saga = await _dbContext.OrderSagas
            .FirstOrDefaultAsync(s => s.OrderId == message.OrderId);

        if (saga == null) return;

        saga.InventoryReserved = true;
        saga.CurrentState = "PaymentProcessing";
        await _dbContext.SaveChangesAsync();

        await _publisher.PublishAsync("process.payment",
            new ProcessPayment { OrderId = message.OrderId });
    }

    // Step 3: Payment Processed
    [CapSubscribe("payment.processed")]
    public async Task OnPaymentProcessed(PaymentProcessed message)
    {
        var saga = await _dbContext.OrderSagas
            .FirstOrDefaultAsync(s => s.OrderId == message.OrderId);

        if (saga == null) return;

        saga.PaymentProcessed = true;
        saga.CurrentState = "Completed";
        saga.CompletedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        await _publisher.PublishAsync("order.completed",
            new OrderCompleted { OrderId = message.OrderId });
    }

    // Compensation (if needed)
    [CapSubscribe("inventory.reservation.failed")]
    public async Task OnInventoryReservationFailed(InventoryReservationFailed message)
    {
        var saga = await _dbContext.OrderSagas
            .FirstOrDefaultAsync(s => s.OrderId == message.OrderId);

        if (saga == null) return;

        saga.CurrentState = "Failed";
        await _dbContext.SaveChangesAsync();

        // Rollback actions...
    }
}
```

## Phase 5: Testing (Week 3-4)

### Unit Tests
```csharp
public class OrderSubmittedHandlerTests
{
    [Fact]
    public async Task Should_Process_Order_Successfully()
    {
        // Arrange
        var publisher = Substitute.For<ICapPublisher>();
        var handler = new OrderSubmittedHandler(_logger, publisher);

        // Act
        await handler.Handle(new OrderSubmitted { OrderId = Guid.NewGuid() });

        // Assert
        await publisher.Received(1).PublishAsync(
            "inventory.reserved",
            Arg.Any<InventoryReserved>());
    }
}
```

### Integration Tests
```csharp
public class OrderFlowIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Should_Complete_Order_Flow()
    {
        // Test full order flow with CAP
    }
}
```

## Phase 6: Deployment (Week 4)

### Rollout Strategy
1. **Parallel Run**: Deploy CAP alongside MassTransit
2. **Shadow Mode**: Publish to both systems, consume only from MassTransit
3. **Gradual Migration**: Move consumers one by one to CAP
4. **Full Switch**: Disable MassTransit
5. **Cleanup**: Remove MassTransit packages

### Monitoring
```csharp
// CAP Dashboard
app.UseCap();
// Access at: http://localhost:5000/cap
```

## Migration Checklist

- [ ] CAP packages installed
- [ ] CAP configured with PostgreSQL + RabbitMQ/Kafka
- [ ] Database tables created
- [ ] All consumers converted to CAP subscribers
- [ ] All publishers converted to CAP publishers
- [ ] Saga state machines implemented
- [ ] Unit tests updated
- [ ] Integration tests passing
- [ ] Dashboard configured
- [ ] Monitoring/alerting setup
- [ ] Documentation updated
- [ ] Team training completed
- [ ] Production deployment plan ready
- [ ] Rollback plan documented

## Benefits After Migration

✅ **No licensing fees** for v9+ features
✅ **Simpler codebase** (less abstraction)
✅ **Built-in dashboard** for monitoring
✅ **Better transaction handling** with EF Core
✅ **Active maintenance** and community support

## Considerations

⚠️ **No built-in Saga** - Need manual implementation
⚠️ **Different API** - Learning curve for team
⚠️ **Migration effort** - 3-4 weeks estimated

## Resources

- CAP Documentation: https://cap.dotnetcore.xyz/
- CAP GitHub: https://github.com/dotnetcore/CAP
- CAP Examples: https://github.com/dotnetcore/CAP/tree/master/samples
