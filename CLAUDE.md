# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Repository Is

PlatformKernel is a SharedKernel monorepo: a collection of independently consumable infrastructure libraries (`SharedKernel.*`) for .NET microservices, plus reference blueprints and a TimescaleDB demo. The libraries are consumed by downstream services as NuGet-style project references. There is no single runnable application here — the unit of work is an individual `SharedKernel.[ModuleName]` library.

---

## Critical Constraints

- **.NET 8.0 only** — do NOT upgrade to .NET 9 or 10. Locked by MassTransit compatibility.
- **MassTransit pinned at 8.2.3** — do NOT upgrade to v9+. Commercial licensing concern. See `docs/adr-001-keep-masstransit-8x.md`.
- **`TreatWarningsAsErrors=true` and `EnforceCodeStyleInBuild=true`** are set globally in `Directory.Build.props`. Fix or suppress all warnings — zero tolerance.
- **Docker services required** for integration tests: PostgreSQL, Redis, RabbitMQ, Seq, MinIO must be running.
- **SonarQube enforces ≥55% code coverage** as a quality gate.

---

## Build & Test Commands

```bash
# Start required infrastructure
docker compose up -d

# Build (full workflow: clean → restore → build → CVE audit)
task build
# or
dotnet restore && dotnet build --no-restore -warnaserror

# Run all tests
dotnet test

# Run a specific module's tests
dotnet test src/shared-kernel/SharedKernel.Entity.Test/SharedKernel.Entity.Test.csproj

# Run with coverage (OpenCover format for SonarQube)
dotnet test --collect:"XPlat Code Coverage;Format=opencover" --no-build

# Clean all artifacts (bin, obj, TestResults, .sonarqube)
task clean

# SonarQube analysis (requires dotnet-sonarscanner, coverlet.console, dotnet-coverage)
./sonar-analysis.sh          # Linux/macOS
./SonarAnalysis.ps1          # Windows
```

---

## Architecture

### Module Structure

Every `SharedKernel.[ModuleName]` follows this layout:

```
SharedKernel.[ModuleName]/
  Abstractions/     # Interfaces only (I[Name]Service.cs)
  Extensions/       # IServiceCollection extension classes
  Services/         # Implementations
  Options/          # Configuration classes
  Models/           # DTOs, value objects
  Internal/         # Private helpers, not exposed
  SharedKernel.[ModuleName].csproj

SharedKernel.[ModuleName].Test/          # xUnit unit tests
SharedKernel.[ModuleName].Test.Service/  # Integration tests (full ASP.NET host)
SharedKernel.[ModuleName].Test.AppHost/  # .NET Aspire test hosts (where applicable)
```

**DI registration**: every module exposes a single `Add{Module}()` extension on `IServiceCollection`:
```csharp
services.AddDistributedCache(configuration);
services.AddMassTransitCustom(configuration, configure: bus => { ... });
```

**No cross-module dependencies** — modules must be independently usable. The only permitted exceptions are `SharedKernel.Entity` and `SharedKernel.Exception`, which are foundational.

### Entity Hierarchy (`SharedKernel.Entity`)

- `BaseEntity` — soft-delete (`IsDeleted`), audit fields (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `DeletedAt`, `DeletedBy`), `Clone()`
- `EntityData : BaseEntity` — adds `long Id` (identity PK) and `string? Code` (auto-GUID). Most domain entities extend this.
- `BaseAuditLog` — event sourcing log with `EventType`, `AggregateCode`, `ChangedFields`

**All audit timestamps are Unix `long` values, not `DateTime`.**

### CQRS (`SharedKernel.CQRS`)

Two parallel namespaces with identical interfaces — pick one consistently within a service:
- `SharedKernel.CQRS.MediatR.*` — legacy (MediatR)
- `SharedKernel.CQRS.Mediator.*` — preferred (martinothamar/Mediator)

Key interfaces:

| Interface | Role |
|---|---|
| `ICommandBase<TResponse>` | State-modifying command |
| `ICreateCommandBase<TModel, TResponse>` | Create command; requires `TModel Payload` |
| `IUpdateCommandBase<TModel, TResponse>` | Update command; requires `TModel Payload` |
| `IDeleteCommandBase<TModel, TResponse>` | Delete command; requires `TModel Payload` |
| `ICommandHandlerBase<TCommand, TResponse>` | Implement `HandleAsync` + override `RemoveCaches` |
| `IQueryBase<TResponse>` | Returns `(IHeaderDictionary, TResponse)` — headers carry pagination metadata |
| `IQuerySingleBase<TResponse>` | Single-item query |
| `IQueryListBase<TResponse>` | List query |
| `IQueryPagedBase<TResponse>` | Paged query; has `IPageable Pageable` property |

### Repository & Unit of Work

- `IGenericRepository<T>` — CRUD: `GetByIdAsync`, `GetPagedAsync`, `GetQueryable()`
- `IFluentRepository<T> : IGenericRepository<T>` — spec-aware: `GetOneAsync(spec)`, `GetAllAsync(gridSpec)`, `CountAsync(gridSpec)`
- `AppDbContextBase<TContext>` — base EF Core DbContext; calls `ApplyConfigurationsFromAssembly` and registers custom DB functions (JSONB, timezone, date extensions)
- Always use `.AsNoTracking()` for read-only queries; avoid N+1 via `.Include()`

### Specification Pattern (`SharedKernel.Specification`)

Extract all query filtering into specifications instead of raw LINQ in repositories:
- Extend `SpecificationBase<T>` for single-entity queries
- Extend `GridSpecificationBase<T>` for paged/filterable list queries
- Compose with `AndSpec<T>`, `OrSpec<T>`, `NegatedSpec<T>`

### Transactional Outbox (Messaging)

**All integration events must use the outbox pattern** — write the entity and publish the event in the same EF Core transaction. Use `SharedKernel.MassTransit` (RabbitMQ or Kafka transport) or `SharedKernel.Rebus`.

MassTransit configuration reads from `appsettings.json`:
```json
{
  "MessageQueueSettings": {
    "QueueType": "RabbitMq",
    "RabbitMqOptions": { "Url": "...", "Username": "...", "Password": "..." }
  }
}
```
Set `"QueueType": "Kafka"` and provide `KafkaOptions.Host` to switch transports.

### Pagination (`SharedKernel.Pagination`)

Spring-style pagination: `Pageable.Of(pageNumber, pageSize)`. Query handlers return `(IHeaderDictionary, TResponse)` tuples where the header dictionary carries pagination metadata.

### Bulk Insert (`SharedKernel.BulkInsert`)

Use `IFluentBulkInsertService<TEntity>` or `IPostgreSqlBulkInsertService` for batch writes — never per-record `SaveChangesAsync` loops for bulk data.

---

## Code Conventions

- **Visibility**: types are `internal sealed` by default. Widen scope only when justified.
- **Explicit typing**: use `var` only when the type is evident from the right-hand side.
- **Null checks**: `is null` / `is not null` — never `== null`.
- **Nullable**: nullable reference types are enabled everywhere. Handle nulls explicitly.
- **Async-first**: all I/O uses `async`/`await` with `CancellationToken`.
- **Records for DTOs**: prefer `record` for immutable data transfer objects.
- **Use `TryAdd*`** when registering DI services to prevent duplicate registrations.
- **No business logic in SharedKernel modules** — infrastructure and cross-cutting concerns only.
- **No `// TODO` comments** in committed code.
- **No static mutable state** that persists between requests.

---

## Adding a New SharedKernel Module

1. Create the library project and test project using the standard folder layout above.
2. Implement abstractions first (`Abstractions/`), then services, then the DI extension.
3. Add **both** projects to `PlatformKernel.sln`.
4. Write unit tests (xUnit `[Fact]`/`[Theory]`) and integration tests with Testcontainers (no mocks for infrastructure).
5. Add a `README.md` with a usage example and the configuration section name.

Integration test fixture pattern:
```csharp
public sealed class MyContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder().Build();
    public string ConnectionString => _container.GetConnectionString();
    public Task InitializeAsync() => _container.StartAsync();
    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}

[CollectionDefinition("MyCollection")]
public sealed class MyCollection : ICollectionFixture<MyContainerFixture> { }
```

---

## Git Conventions

Semantic commits: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`
- Subject line ≤50 characters, imperative mood (`Add`, not `Added`), capitalized, no trailing period.
- Separate body from subject with a blank line.

---

## Infrastructure Services (docker-compose.yml)

| Service | URL |
|---|---|
| pgAdmin | http://localhost:5050 |
| Redis Commander | http://localhost:8081 |
| Seq (logs) | http://localhost:5341 |
| MinIO Console | http://localhost:9011 |
| RabbitMQ Management | http://localhost:15672 |

Environment variables are loaded from a `.env` file in the project root (not committed). See `README.md` for the full variable list.
