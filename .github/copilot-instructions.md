# Platform Kernel Development Guidelines

## Build and Test

```bash
# Build (requires Task CLI: brew install go-task/tap/go-task)
task build          # Full workflow: clean → restore → build → check vulnerabilities
make build          # Alternative using Make

# Test with coverage
dotnet test --collect:"XPlat Code Coverage;Format=opencover" --no-build

# Code quality analysis (requires Docker services running)
./sonar-analysis.sh           # Linux/macOS
./SonarAnalysis.ps1          # Windows

# Docker services (REQUIRED for local development)
docker-compose up -d         # Start PostgreSQL, Redis, RabbitMQ, Seq, MinIO
```

## Architecture

**SharedKernel pattern**: Reusable infrastructure modules consumed by all services. Each module follows `Add{Module}()` extension method pattern for DI registration.

**CQRS with dual providers**: Use MediatR (legacy) or Mediator (modern). Commands modify state (`ICommandBase<TResponse>`), queries read data (`IQueryBase<TResponse>`). Handlers implement `ICommandHandlerBase`/`IQueryHandlerBase`.

**Transactional Outbox**: All integration events MUST use MassTransit/Rebus outbox pattern. Write entity + publish event in same transaction. See modules like SharedKernel.MassTransit for implementation.

**Specifications**: Extract query logic from repositories using `ISpecification<T>`. Compose with `AndSpec`/`OrSpec`. See SharedKernel.Specification.

Key architectural decisions documented in [docs/](docs/):
- [ADR-001](docs/adr-001-keep-masstransit-8x.md): MassTransit 8.x version lock
- [Messaging alternatives](docs/messaging-alternatives-comparison.md)
- [TimescaleDB guide](docs/timescaledb-guide.md)

## Code Conventions

**Language**: C# 12 with .NET 8.0 (target framework locked, do NOT upgrade to .NET 10)

**Strict compilation**: `TreatWarningsAsErrors=true` and `EnforceCodeStyleInBuild=true`. Fix warnings immediately or suppress in Directory.Build.props. Follow .editorconfig rules.

**Typing**: Explicit types preferred. Only use `var` when type is evident. Use `is null` checks instead of `== null`.

**Nullability**: Nullable reference types enabled. Handle nulls explicitly.

**Async-first**: All I/O operations use `async`/`await` with `CancellationToken` parameters.

**Access modifiers**: Types are `internal sealed` by default unless explicitly stated otherwise.

**Repositories**: Reference SharedKernel.RepositoryBase for `IGenericRepository<T>` and `IFluentRepository` patterns. See existing SharedKernel projects for examples.

**Pagination**: Use Spring-style pagination from SharedKernel.Pagination (`IPageable`, `IPage<T>`). Queries return `(IHeaderDictionary, TResponse)` tuple for header metadata.

**Soft Delete**: Entities implement `ILogicalDelete` with `IsDeleted` flag. Query filters exclude deleted by default.

**Auditing**: Use `BaseEntity`/`BaseAuditLog` for CreatedAt/UpdatedBy/DeletedBy tracking.

## Critical Constraints

⚠️ **MassTransit locked at 8.2.3**: Do NOT upgrade to v9+. Intentional decision to avoid commercial licensing. Pin exact versions when adding MassTransit packages.

⚠️ **.NET 8.0 only**: Cannot upgrade to .NET 10 due to MassTransit constraint.

⚠️ **Docker services required**: Tests and services fail without PostgreSQL, Redis, RabbitMQ, Seq, MinIO running locally.

⚠️ **SonarQube quality gates**: Minimum 55% code coverage enforced. Install required tools: `dotnet-sonarscanner`, `coverlet.console`, `dotnet-coverage`.

⚠️ **Git conventions**: Follow semantic commits (`feat`, `fix`, `docs`, `refactor`, `test`, `chore`). See [README Git Convention](README.md#git-convention) section.

## Testing

Write unit tests for business logic and integration tests for API endpoints. Test projects follow `{Module}.Test` naming pattern.

Coverage reports in OpenCover format for SonarQube integration.

## Environment Setup

Install VS Code extensions listed in [README Environment Setup](README.md#development-tools):
- C# Dev Kit
- SonarLint
- EditorConfig
- REST Client
- Code Spell Checker

## References

- Full project structure: [README Project Structure](README.md#project-structure)
- Code review checklist: [README Code Review](README.md#code-review-checklist)
- Clean architecture rules: [clean-architecture/.cursorrules](src/blueprint-service/clean-architecture/.cursorrules)
