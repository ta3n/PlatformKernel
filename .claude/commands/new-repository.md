Scaffold a repository for an entity. Requires one argument: the entity name.

Usage: `/new-repository Product`

## Templates

### Interface
```csharp
using SharedKernel.RepositoryBase;

namespace {Namespace}.Domain.Repositories;

public interface I{Entity}Repository : IFluentRepository<{Entity}>
{
    // add domain-specific query methods here (avoid leaking EF Core into domain)
}
```

### Implementation
```csharp
using Microsoft.EntityFrameworkCore;
using SharedKernel.RepositoryBase.Implementations;

namespace {Namespace}.Infrastructure.Data.Repositories;

internal sealed class {Entity}Repository(
    ApplicationDbContext context
) : FluentRepositoryBase<{Entity}>(context), I{Entity}Repository
{
    // override only when the base IFluentRepository<T> methods are insufficient
}
```

### DI registration (in Infrastructure/DependencyInjection.cs)
```csharp
services.TryAddScoped<I{Entity}Repository, {Entity}Repository>();
```

## Base repository capabilities (already available via IFluentRepository<T>)

| Method | Usage |
|---|---|
| `GetByIdAsync(id)` | Fetch single by PK |
| `GetPagedAsync(pageable)` | Paginated without spec |
| `GetQueryable()` | Raw IQueryable (use sparingly) |
| `GetOneAsync(spec)` | Single entity matching a `SpecificationBase<T>` |
| `GetAllAsync(gridSpec)` | Filtered + paged via `GridSpecificationBase<T>` |
| `CountAsync(gridSpec)` | Count matching records |
| `AddAsync(entity)` | Insert (call `CommitAsync` to persist) |
| `UpdateAsync(entity)` | Update (call `CommitAsync` to persist) |
| `DeleteAsync(entity)` | Soft-delete via `IsDeleted = true` |

## Rules

- Always use `.AsNoTracking()` in read-only query methods
- Never expose `IQueryable` through the interface — use specifications instead
- Repository interface lives in `Domain/Repositories/`, implementation in `Infrastructure/Data/Repositories/`
- Register with `TryAddScoped` (not `AddScoped`) to prevent duplicate registrations
- Implementation class is `internal sealed`
