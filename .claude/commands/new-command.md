Scaffold a CQRS command + handler. Requires arguments: entity name and operation type.

Usage:
- `/new-command Product create` → ICreateCommandBase + CreateCommandHandlerBase
- `/new-command Product update` → IUpdateCommandBase + UpdateCommandHandlerBase
- `/new-command Product delete` → IDeleteCommandBase + DeleteCommandHandlerBase
- `/new-command Product action <ActionName>` → ICommandBase + ActionCommandHandlerBase

## Provider choice

Default to `SharedKernel.CQRS.Mediator.*` (modern). Use `SharedKernel.CQRS.MediatR.*` only if the target service already uses MediatR.

## Templates

### Create command
```csharp
using SharedKernel.CQRS.Mediator.BaseCommand;

namespace {Namespace}.Application.Commands.{Entity};

public sealed class Create{Entity}Command : ICreateCommandBase<{Entity}Dto, {Entity}Dto>
{
    public required {Entity}Dto Payload { get; set; }
}
```

### Create handler
```csharp
using SharedKernel.CQRS.Mediator.BaseCommand.Implementations;

namespace {Namespace}.Application.Commands.{Entity};

internal sealed class Create{Entity}CommandHandler(
    I{Entity}Repository repository
) : CreateCommandHandlerBase<Create{Entity}Command, {Entity}Dto>
{
    protected override async Task<{Entity}Dto> HandleAsync(
        Create{Entity}Command request,
        CancellationToken cancellationToken)
    {
        // implementation
        throw new NotImplementedException();
    }

    protected override void RemoveCaches(Create{Entity}Command request)
    {
        // invalidate related cache keys here
    }
}
```

### Update command
```csharp
public sealed class Update{Entity}Command : IUpdateCommandBase<{Entity}Dto, {Entity}Dto>
{
    public required {Entity}Dto Payload { get; set; }
}
```

### Delete command
```csharp
public sealed class Delete{Entity}Command : IDeleteCommandBase<{Entity}Dto, bool>
{
    public required {Entity}Dto Payload { get; set; }
}
```

### Action command (custom operation)
```csharp
public sealed class {ActionName}{Entity}Command : ICommandBase<bool>
{
    public required long Id { get; set; }
}
```

## Rules

- Command classes are `sealed` records or classes — use `sealed class` for commands with `Payload`, `sealed record` for simple ones
- Handler classes are `internal sealed`
- Always override `RemoveCaches` on mutating handlers to invalidate query caches
- Never put business logic in the command class — only data properties
- Use primary constructors for dependency injection in handlers
- Handlers live in `Application/Commands/{Entity}/` folder
- Place command and handler in the same folder, separate files
