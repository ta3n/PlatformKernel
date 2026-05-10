Scaffold a CQRS query + handler. Requires arguments: entity name and query type.

Usage:
- `/new-query Product single` → IQuerySingleBase + QuerySingleBaseHandler (get by ID)
- `/new-query Product list` → IQueryListBase + QueryListBaseHandler (get all)
- `/new-query Product paged` → IQueryPagedBase + QueryPageBaseHandler (paginated)

## Provider choice

Default to `SharedKernel.CQRS.Mediator.*` (modern). Use `SharedKernel.CQRS.MediatR.*` only if the target service already uses MediatR.

## Templates

### Single query (get by ID)
```csharp
using SharedKernel.CQRS.Mediator.BaseQuery;

namespace {Namespace}.Application.Queries.{Entity};

public sealed class Get{Entity}ByIdQuery : IQuerySingleBase<{Entity}Dto>
{
    public required long Id { get; set; }
}
```

### Single handler
```csharp
using Microsoft.AspNetCore.Http;
using SharedKernel.Cache.Services;
using SharedKernel.CQRS.Mediator.BaseQuery.Implementations;

namespace {Namespace}.Application.Queries.{Entity};

internal sealed class Get{Entity}ByIdQueryHandler(
    I{Entity}Repository repository,
    ICacheService cacheService
) : QuerySingleBaseHandler<Get{Entity}ByIdQuery, {Entity}Dto>(cacheService)
{
    protected override async Task<(IHeaderDictionary, {Entity}Dto)> HandleAsync(
        Get{Entity}ByIdQuery request,
        CancellationToken cancellationToken)
    {
        // implementation — use .AsNoTracking() for read-only queries
        throw new NotImplementedException();
    }

    protected override string? GetCacheKey(Get{Entity}ByIdQuery request)
        => $"{nameof({Entity})}:{request.Id}";
}
```

### Paged query
```csharp
using SharedKernel.CQRS.Mediator.BaseQuery;
using SharedKernel.Pagination;

namespace {Namespace}.Application.Queries.{Entity};

public sealed class Get{Entity}PagedQuery : IQueryPagedBase<{Entity}Dto>
{
    public required IPageable Pageable { get; set; }
    // add filter properties here
}
```

### Paged handler
```csharp
internal sealed class Get{Entity}PagedQueryHandler(
    I{Entity}Repository repository
) : QueryPageBaseHandler<Get{Entity}PagedQuery, {Entity}Dto>
{
    protected override async Task<(IHeaderDictionary, IEnumerable<{Entity}Dto>)> HandleAsync(
        Get{Entity}PagedQuery request,
        CancellationToken cancellationToken)
    {
        // use GetAllAsync(gridSpec) with a GridSpecificationBase
        throw new NotImplementedException();
    }
}
```

## Rules

- Query return type is always `(IHeaderDictionary, TResponse)` — headers carry pagination metadata
- Always use `.AsNoTracking()` for EF Core reads in query handlers
- Inject `ICacheService` and override `GetCacheKey()` for cacheable queries
- For paged queries, use `IFluentRepository.GetAllAsync(gridSpec)` + `GridSpecificationBase`
- Never use raw LINQ in handlers — extract filtering into specifications
- Handlers are `internal sealed`, queries are `public sealed`
- Place query and handler in the same folder: `Application/Queries/{Entity}/`
