using Liberty.ApplicationShared.Utils;
using Liberty.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Liberty.UnitOfWork.Interceptors;

/// <summary>
/// The <c>AuditSaveChangesInterceptor</c> class is a custom interceptor that
/// extends the <c>SaveChangesInterceptor</c> functionality provided by Entity Framework Core.
/// It adds auditing capabilities by handling and updating metadata for entities on save operations.
/// </summary>
/// <remarks>
/// This interceptor is designed to automatically apply audit information for entities
/// that derive from <c>BaseEntity</c>. Audit information such as created/modified/deleted
/// by and timestamps are set during the save operation.
/// </remarks>
/// <example>
/// This class is intended to be registered as part of the Entity Framework Core pipeline
/// to handle save operations, typically for audit-logging purposes.
/// </example>
/// <threadsafety>
/// This class relies on the provided <c>IHttpContextAccessor</c> to retrieve information about
/// the current user performing the operation. Ensure thread safety when using HttpContextAccessor.
/// </threadsafety>
/// <dependencies>
/// - <c>IHttpContextAccessor</c>: Required to access user-related information from the HTTP context.
/// - <c>BaseEntity</c>: Entities in the DbSet should derive from this class to benefit from auditing.
/// </dependencies>
public class AuditSaveChangesInterceptor(
    IHttpContextAccessor httpContextAccessor
) : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts the saving changes operation for the DbContext to apply custom logic,
    /// such as auditing modified, created, or deleted entities.
    /// </summary>
    /// <param name="eventData">Provides information and context about the DbContext operation.</param>
    /// <param name="result">The result of the save operation, which can be altered by the interceptor.</param>
    /// <returns>The interception result of the saving operation, allowing the SaveChanges operation to proceed as modified or as-is.</returns>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context != null)
        {
            SetEntitiesAudit(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepts the asynchronous save changes operation to apply audit logic before saving changes to the database.
    /// </summary>
    /// <param name="eventData">Provides context information about the currently tracked changes in the database operation.</param>
    /// <param name="result">Represents the result of the save changes operation, which may be intercepted or modified.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous save operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the intercepted or original result of the save changes process.</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context != null)
        {
            SetEntitiesAudit(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Sets audit-related information for entities being tracked by the provided database context.
    /// </summary>
    /// <param name="dbContext">The database context containing the tracked entities.</param>
    private void SetEntitiesAudit(
        DbContext dbContext
    )
    {
        var entities = dbContext.ChangeTracker
            .Entries()
            .Where(
                x => x is
                {
                    Entity: BaseEntity, State: EntityState.Added or EntityState.Modified or EntityState.Deleted
                }
            );

        var entityEntries = entities as EntityEntry[] ?? [.. entities];
        if (entityEntries is { Length: <= 0 })
        {
            return;
        }

        var modifiedOrCreatedBy = httpContextAccessor.HttpContext?.User.Identity?.Name;
        if (string.IsNullOrEmpty(modifiedOrCreatedBy))
        {
            modifiedOrCreatedBy = httpContextAccessor.HttpContext?.User.FindFirst("code")?.Value ?? "System";
        }

        foreach (var entityEntry in entityEntries)
        {
            AuditHandle(dbContext, entityEntry, modifiedOrCreatedBy);
        }
    }

    /// <summary>
    /// Updates auditing properties of the given entity based on its state (Added, Modified, or Deleted).
    /// </summary>
    /// <param name="dbContext">
    /// The instance of the database context responsible for tracking the entity.
    /// </param>
    /// <param name="entityEntry">
    /// The entry representing the entity being audited, holding state and property change information.
    /// </param>
    /// <param name="modifiedOrCreatedBy">
    /// The identifier for the user or system responsible for the current modification or creation.
    /// </param>
    private static void AuditHandle(
        DbContext dbContext,
        EntityEntry entityEntry,
        string modifiedOrCreatedBy
    )
    {
        var longDate = ConvertUtil.ToLong(
            ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff")
        );

        switch (entityEntry.State)
        {
            case EntityState.Added:
                if (((BaseEntity)entityEntry.Entity).DisplayOrder <= 0)
                {
                    ((BaseEntity)entityEntry.Entity).DisplayOrder = longDate;
                }

                ((BaseEntity)entityEntry.Entity).CreatedAt = longDate;
                ((BaseEntity)entityEntry.Entity).CreatedBy = modifiedOrCreatedBy;

                break;
            case EntityState.Modified:
                ((BaseEntity)entityEntry.Entity).UpdatedAt = longDate;
                ((BaseEntity)entityEntry.Entity).UpdatedBy = modifiedOrCreatedBy;

                if (((BaseEntity)entityEntry.Entity).IsDeleted)
                {
                    ((BaseEntity)entityEntry.Entity).DeletedAt = longDate;
                    ((BaseEntity)entityEntry.Entity).DeletedBy = modifiedOrCreatedBy;
                }

                break;
            case EntityState.Deleted:
                ((BaseEntity)entityEntry.Entity).DeletedAt = longDate;
                ((BaseEntity)entityEntry.Entity).DeletedBy = modifiedOrCreatedBy;

                break;
            default:
                dbContext.Entry((BaseEntity)entityEntry.Entity).Property(x => x.CreatedAt).IsModified = false;
                dbContext.Entry((BaseEntity)entityEntry.Entity).Property(x => x.CreatedBy).IsModified = false;

                break;
        }
    }
}
