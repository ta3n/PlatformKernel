using Liberty.ApplicationShared.Utils;
using Liberty.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Liberty.UnitOfWork;

public class AppDbContextBase<T>(
    DbContextOptions options,
    IHttpContextAccessor httpContextAccessor
) : DbContext(options)
    where T : DbContext
{
    protected override void OnModelCreating(
        ModelBuilder builder
    )
    {
        builder.ApplyConfigurationsFromAssembly(typeof(T).Assembly);
    }

    public override int SaveChanges(
        bool acceptAllChangesOnSuccess
    )
    {
        SetEntitiesAudit();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new()
    )
    {
        SetEntitiesAudit();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected void SetEntitiesAudit()
    {
        var entities = ChangeTracker
            .Entries()
            .Where(
                x => x is
                {
                    Entity: BaseEntity, State: EntityState.Added or EntityState.Modified or EntityState.Deleted
                }
            );

        var entityEntries = entities as EntityEntry[] ?? entities.ToArray();
        if (entityEntries is { Length: <= 0 })
        {
            return;
        }

        var modifiedOrCreatedBy = httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System";

        foreach (var entityEntry in entityEntries)
        {
            var longDate = ConvertUtil.ToLong(
                ConvertUtil.ToString(DateTime.Now, "yyyyMMddHHmmssfff")
            );

            switch (entityEntry.State)
            {
                case EntityState.Added:
                    ((BaseEntity)entityEntry.Entity).DisplayOrder = longDate;
                    ((BaseEntity)entityEntry.Entity).CreatedAt = longDate;
                    ((BaseEntity)entityEntry.Entity).CreatedBy = modifiedOrCreatedBy;
                    ((BaseEntity)entityEntry.Entity).IsEnabled = true;
                    ((BaseEntity)entityEntry.Entity).IsVisible = true;

                    break;
                case EntityState.Modified:
                    ((BaseEntity)entityEntry.Entity).UpdatedAt = longDate;
                    ((BaseEntity)entityEntry.Entity).UpdatedBy = modifiedOrCreatedBy;

                    break;
                case EntityState.Deleted:
                    ((EntityData)entityEntry.Entity).Delete();
                    ((BaseEntity)entityEntry.Entity).DeletedAt = longDate;
                    ((BaseEntity)entityEntry.Entity).DeletedBy = modifiedOrCreatedBy;
                    entityEntry.State = EntityState.Modified;

                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    Entry((EntityData)entityEntry.Entity).Property(x => x.CreatedAt).IsModified = false;
                    Entry((EntityData)entityEntry.Entity).Property(x => x.CreatedBy).IsModified = false;

                    break;
            }
        }
    }
}
