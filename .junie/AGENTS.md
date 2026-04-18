# AGENTS.md — PlatformKernel

SharedKernel mono-repo providing reusable infrastructure libraries for .NET microservices. Each `SharedKernel.*` module is an independently consumable NuGet-style library with a DI registration extension method.

## ⚠️ Critical Constraints

- **.NET 8.0 only** — do NOT upgrade to .NET 9/10 (MassTransit compatibility)
- **MassTransit pinned at 8.2.3** — do NOT upgrade to v9+ (commercial licensing — see `docs/adr-001-keep-masstransit-8x.md`)
- **`TreatWarningsAsErrors=true`** and **`EnforceCodeStyleInBuild=true`** — fix or suppress all warnings in `Directory.Build.props`
- **Docker services required** for tests: `docker compose up -d` (PostgreSQL, Redis, RabbitMQ, Seq, MinIO)
- **55% code coverage** minimum enforced by SonarQube quality gate

## Build & Test

### Environment Setup
1. **Docker Infrastructure**: Required for most integration tests.
   ```bash
   docker compose up -d
   ```
   Starts PostgreSQL, Redis, RabbitMQ, Seq, and MinIO.

2. **Tooling**:
   - `task` (Taskfile.yml) or `make` (Makefile) for common workflows.
   - .NET 8 SDK (pinned in `global.json`).

### Build Commands
```bash
task build                               # Full workflow: clean → restore → build → CVE audit
# OR
dotnet build                             # Standard .NET build
```

### Testing
- **Unit Tests**: Co-located with modules in `src/shared-kernel/*.Test`.
- **Integration Tests**: `*.Test.Service` projects.
- **Test Execution**:
  ```bash
  # Run all tests
  dotnet test

  # Run tests for a specific project
  dotnet test src/shared-kernel/SharedKernel.Entity.Test/SharedKernel.Entity.Test.csproj

  # Run with coverage
  dotnet test --collect:"XPlat Code Coverage;Format=opencover" --no-build
  ```

### Adding a New Test
Create a class in the corresponding `.Test` project. Use `Xunit` and `Fact`/`Theory` attributes.
**Example (`src/shared-kernel/SharedKernel.Entity.Test/DemoTest.cs`):**
```csharp
using Xunit;
using SharedKernel.Entity;

namespace SharedKernel.Entity.Test;

public class DemoTest
{
    [Fact]
    public void BaseEntity_ShouldHaveDefaultValues()
    {
        var entity = new BaseEntity();

        Assert.False(entity.IsEnabled);
        Assert.True(entity.IsVisible);
        Assert.False(entity.IsDeleted);
        Assert.Equal(0, entity.DisplayOrder);
    }
}
```

## Code Conventions

- **Visibility**: Types are `internal sealed` by default.
- **Types**: Use explicit types unless the assignment is obvious (e.g., `new()`).
- **Null Safety**: Use `is null` / `is not null`. Nullable reference types are enabled.
- **Async**: All I/O must be `async` with `CancellationToken`.
- **Immutability**: Prefer `record` for DTOs.
- **Project Configuration**: `TreatWarningsAsErrors` and `EnforceCodeStyleInBuild` are `true` in `Directory.Build.props`. All SonarQube warnings must be addressed or explicitly suppressed.

## Architecture

**SharedKernel pattern**: Every module exposes `Add{Module}()` on `IServiceCollection`:
```csharp
services.AddDistributedCache(configuration);
services.AddMassTransitCustom(configuration, configure: bus => { ... });
```

**CQRS — dual provider**: Two parallel namespaces with identical interfaces:
- `SharedKernel.CQRS.MediatR.*` — legacy, uses MediatR
- `SharedKernel.CQRS.Mediator.*` — modern alternative

Key interfaces (both providers):
| Interface | Role |
|---|---|
| `ICommandBase<TResponse>` | State-modifying command |
| `ICreateCommandBase<TModel, TResponse>` | Create command with `Payload` property |
| `IUpdateCommandBase<TModel, TResponse>` | Update command with `Payload` property |
| `IDeleteCommandBase<TModel, TResponse>` | Delete command with `Payload` property |
| `ICommandHandlerBase<TCommand, TResponse>` | Implement `HandleAsync` + override `RemoveCaches` |
| `IQueryBase<TResponse>` | Returns `(IHeaderDictionary, TResponse)` — headers carry pagination metadata |
| `IQueryPagedBase<TResponse>` | Has `IPageable Pageable` property |
| `IQueryHandlerBase<TQuery, TResponse>` | Query handler base |

**Transactional Outbox**: All integration events MUST use MassTransit/Rebus outbox. Write entity + publish event in the same EF Core transaction (`SharedKernel.MassTransit` or `SharedKernel.Rebus`). See also `docs/hybrid-rebus-cap-analysis.md` and `docs/masstransit-to-cap-migration.md` for migration considerations.

**Repositories** (`SharedKernel.RepositoryBase`):
- `IGenericRepository<T>` — basic CRUD with `GetByIdAsync`, `GetPagedAsync`, `GetQueryable()`
- `IFluentRepository<T> : IGenericRepository<T>` — specification-aware: `GetOneAsync(spec)`, `GetAllAsync(gridSpec)`, `CountAsync(gridSpec)`

**Specifications** (`SharedKernel.Specification`): Extract query logic from repositories. Extend `SpecificationBase<T>` or `GridSpecificationBase<T>`. Compose with `AndSpec<T>`, `OrSpec<T>`, `NegatedSpec<T>`.

**Pagination** (`SharedKernel.Pagination`): Spring-style — `Pageable.Of(pageNumber, pageSize)` → `IPage<T>`. Query handlers return `(IHeaderDictionary, TResponse)` tuples.

**Entity hierarchy** (`SharedKernel.Entity`):
- `BaseEntity` — soft-delete (`ILogicalDelete.IsDeleted`), auditing (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `DeletedAt`, `DeletedBy`), `Clone()`
- `BaseAuditLog` — event sourcing log with `EventType`, `AggregateCode`, `ChangedFields`

## Available Modules

All modules live under `src/shared-kernel/`. Key modules by domain:

| Domain | Modules |
|---|---|
| CQRS & Messaging | `SharedKernel.CQRS`, `SharedKernel.MassTransit`, `SharedKernel.Rebus`, `SharedKernel.IntegrationEvent` |
| Data Access | `SharedKernel.RepositoryBase`, `SharedKernel.UnitOfWork`, `SharedKernel.Specification`, `SharedKernel.BulkInsert`, `SharedKernel.BulkInsertPipeline` |
| Entity & Pagination | `SharedKernel.Entity`, `SharedKernel.Pagination`, `SharedKernel.Exception` |
| Caching & Search | `SharedKernel.Cache`, `SharedKernel.ElasticSearch` |
| Observability | `SharedKernel.Serilog`, `SharedKernel.Sentry` |
| Communication | `SharedKernel.Grpc`, `SharedKernel.FirebaseNotification` |
| Infrastructure | `SharedKernel.Hangfire`, `SharedKernel.WorkflowEngineElsa`, `SharedKernel.ServiceDefaults`, `SharedKernel.ServiceBase`, `SharedKernel.AppShared` |


## Key Configuration (appsettings.json)

| Section | Module |
|---|---|
| `Redis` | `SharedKernel.Cache` |
| `MessageQueueSettings.QueueType` (`"RabbitMq"` or `"Kafka"`) | `SharedKernel.MassTransit` |
| `MessageQueueSettings.RabbitMqOptions.*` | RabbitMQ transport |
| `MessageQueueSettings.KafkaOptions.Host` | Kafka transport |

## Project Structure

```
src/shared-kernel/         # All SharedKernel.* library modules + co-located *.Test projects
src/blueprint-service/     # Service scaffold templates (clean-architecture, jhipster)
src/timescaledb-demo/      # TimescaleDB reference implementation
docs/                      # ADRs and architecture guides
docker/                    # Docker Compose fragments (db.yml, sonar.yml)
dependency-audit/          # CVE audit tooling
```

**Test project naming**:
- `*.Test` — unit tests (co-located with module)
- `*.Test.Service` — integration tests requiring running services
- `*.Test.AppHost` — .NET Aspire test hosts

## Git Conventions

Semantic commits: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`
Subject: ≤50 chars, imperative mood, capitalized, no trailing period.

