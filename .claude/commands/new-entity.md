Scaffold a new domain entity. Requires one argument: the entity name (PascalCase).

Usage: `/new-entity Product` → creates `Product.cs` extending `EntityData`

## Output

Generate the following files based on where the user is working (ask if unclear):

### Entity class
```csharp
using SharedKernel.Entity;

namespace {Namespace}.Domain.Entities;

public class {Name} : EntityData
{
    // Properties here — use explicit types, not var
    // All string properties should be nullable (string?) unless required by domain
}
```

### EF Core configuration (IEntityTypeConfiguration)
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace {Namespace}.Infrastructure.Data.Configurations;

internal sealed class {Name}Configuration : IEntityTypeConfiguration<{Name}>
{
    public void Configure(EntityTypeBuilder<{Name}> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(36);
        builder.HasQueryFilter(x => !x.IsDeleted); // global soft-delete filter
    }
}
```

## Rules

- Extend `EntityData` (has `long Id` + `string? Code` auto-GUID) for most domain entities
- Extend `BaseEntity` only if no PK is needed (e.g., join tables)
- All audit timestamps (`CreatedAt`, `UpdatedAt`, `DeletedAt`) are inherited as Unix `long` — never add `DateTime` audit fields
- Soft-delete is handled by `IsDeleted` on `BaseEntity` — never add a separate deleted flag
- `EntityData` constructor sets `IsEnabled = false` and generates a GUID `Code` automatically
- Always add a `HasQueryFilter(x => !x.IsDeleted)` in the EF configuration
- Configuration class must be `internal sealed`
