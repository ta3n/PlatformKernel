Scaffold a Specification for query filtering. Requires arguments: entity name and spec type.

Usage:
- `/new-spec Product single` → SpecificationBase (single-entity filter)
- `/new-spec Product grid` → GridSpecificationBase (paged/list filter)

## Templates

### Single specification
```csharp
using SharedKernel.Specification;

namespace {Namespace}.Infrastructure.Specifications;

internal sealed class {Entity}ByIdSpec : SpecificationBase<{Entity}>
{
    public {Entity}ByIdSpec(long id)
    {
        AddFilter(x => x.Id == id && !x.IsDeleted);
    }
}
```

### Grid specification (for paged/list queries)
```csharp
using SharedKernel.Specification;

namespace {Namespace}.Infrastructure.Specifications;

internal sealed class {Entity}GridSpec : GridSpecificationBase<{Entity}>
{
    public {Entity}GridSpec({Entity}FilterModel filter, IPageable pageable)
    {
        ApplyPaging(pageable);

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            AddFilter(x => x.Code!.Contains(filter.Keyword));
        }

        AddFilter(x => !x.IsDeleted);

        // Default sort
        AddOrderByDescending(x => x.CreatedAt);
    }
}
```

### Filter model
```csharp
namespace {Namespace}.Application.Queries.{Entity};

public sealed record {Entity}FilterModel(
    string? Keyword,
    bool? IsEnabled
);
```

## Composition helpers (from SharedKernel.Specification)

```csharp
// AND / OR / NOT composition
var spec = new {Entity}ByCodeSpec(code)
    .And(new ActiveEntitySpec<{Entity}>());

// Available: AndSpec<T>, OrSpec<T>, NegatedSpec<T>
```

## Rules

- Specifications live in `Infrastructure/Specifications/` — never in repository or handler
- Always filter `!x.IsDeleted` unless intentionally querying deleted records
- `GridSpecificationBase` must call `ApplyPaging(pageable)` for paged results
- Specifications are `internal sealed`
- Use specification composition (`AndSpec`, `OrSpec`) instead of duplicating filter logic
