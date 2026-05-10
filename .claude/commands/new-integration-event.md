Scaffold an integration event with the Transactional Outbox pattern. Requires one argument: the event name.

Usage: `/new-integration-event ProductCreated`

## What to create

### 1. Event contract (in SharedKernel.IntegrationEvent or service's contract project)
```csharp
namespace {Namespace}.IntegrationEvents;

public sealed record {EventName}IntegrationEvent(
    string AggregateCode,
    long OccurredAt
    // add domain-specific properties
);
```

### 2. Publish inside a command handler (outbox pattern)

**MassTransit transport:**
```csharp
using MassTransit;

internal sealed class Create{Entity}CommandHandler(
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint
) : CreateCommandHandlerBase<Create{Entity}Command, {Entity}Dto>
{
    protected override async Task<{Entity}Dto> HandleAsync(
        Create{Entity}Command request,
        CancellationToken cancellationToken)
    {
        var entity = new {Entity} { /* map from request.Payload */ };

        await unitOfWork.{Entity}Repository.AddAsync(entity, cancellationToken);

        // Publish inside the same EF Core transaction via outbox
        await publishEndpoint.Publish(
            new {EventName}IntegrationEvent(entity.Code!, DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            cancellationToken
        );

        await unitOfWork.CommitAsync(cancellationToken);

        return /* map to dto */;
    }
}
```

**Rebus transport (alternative):**
```csharp
using Rebus.Bus;

internal sealed class Create{Entity}CommandHandler(
    IUnitOfWork unitOfWork,
    IBus bus
) : CreateCommandHandlerBase<Create{Entity}Command, {Entity}Dto>
{
    protected override async Task<{Entity}Dto> HandleAsync(
        Create{Entity}Command request,
        CancellationToken cancellationToken)
    {
        var entity = new {Entity} { /* ... */ };
        await unitOfWork.{Entity}Repository.AddAsync(entity, cancellationToken);
        await bus.Publish(new {EventName}IntegrationEvent(entity.Code!, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        await unitOfWork.CommitAsync(cancellationToken);
        return /* map to dto */;
    }
}
```

### 3. Consumer (if this service receives the event)
```csharp
using MassTransit;

internal sealed class {EventName}IntegrationEventConsumer(
    IMediator mediator
) : IConsumer<{EventName}IntegrationEvent>
{
    public async Task Consume(ConsumeContext<{EventName}IntegrationEvent> context)
    {
        // handle the event
        await mediator.Send(new Handle{EventName}Command { /* ... */ }, context.CancellationToken);
    }
}
```

Register the consumer:
```csharp
services.AddMassTransitCustom(configuration, configure: bus =>
{
    bus.AddConsumer<{EventName}IntegrationEventConsumer>();
});
```

## Rules

- **Entity write + event publish must be in the same EF Core transaction** — this is the outbox guarantee
- Always call `CommitAsync` AFTER publishing to the outbox endpoint
- Event records are immutable — use `sealed record` with positional parameters
- `OccurredAt` is always Unix `long` (never `DateTime`)
- Consumer classes are `internal sealed`
- Never publish events after `CommitAsync` — that breaks the outbox guarantee
