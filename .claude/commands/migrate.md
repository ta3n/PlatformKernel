Manage EF Core database migrations. Accepts subcommands.

Usage:
- `/migrate add <MigrationName> [ProjectPath]` — add a new migration
- `/migrate update [ProjectPath]` — apply pending migrations to the database
- `/migrate list [ProjectPath]` — list all migrations and their status
- `/migrate remove [ProjectPath]` — remove the last unapplied migration

## Commands

### Add migration
```bash
# From repo root
dotnet ef migrations add {MigrationName} \
  --project {InfrastructureProject}.csproj \
  --startup-project {WebApiProject}.csproj \
  --output-dir Data/Migrations
```

### Apply migrations
```bash
dotnet ef database update \
  --project {InfrastructureProject}.csproj \
  --startup-project {WebApiProject}.csproj
```

### List migrations
```bash
dotnet ef migrations list \
  --project {InfrastructureProject}.csproj \
  --startup-project {WebApiProject}.csproj
```

### Remove last migration
```bash
dotnet ef migrations remove \
  --project {InfrastructureProject}.csproj \
  --startup-project {WebApiProject}.csproj
```

## DbContext setup reminder

Services should use `AppDbContextBase<TContext>` from `SharedKernel.UnitOfWork`:

```csharp
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : AppDbContextBase<ApplicationDbContext>(options)
{
    public DbSet<Product> Products => Set<Product>();
}
```

`AppDbContextBase` automatically:
- Calls `ApplyConfigurationsFromAssembly` — place all `IEntityTypeConfiguration<T>` in the Infrastructure project
- Registers custom PostgreSQL DB functions (JSONB, timezone, date helpers)

## Rules

- Migration names must be descriptive and PascalCase: `AddProductTable`, `AddIndexOnProductCode`
- Always review the generated migration file before applying — verify it matches your intent
- Docker services (PostgreSQL) must be running: `docker compose up -d`
- Never modify a migration that has already been applied in any environment
