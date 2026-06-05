using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.AuditLogging.Attributes;
using SharedKernel.AuditLogging.Models;
using SharedKernel.AuditLogging.Options;
using SharedKernel.AuditLogging.Services;

namespace SharedKernel.AuditLogging.Interceptors;

public sealed class AuditSaveChangesTrailInterceptor(
    IOptions<AuditLoggingOptions> options,
    IAuditContextAccessor auditContextAccessor,
    IAuditEntityIdResolver entityIdResolver,
    IAuditSink auditSink
) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not null)
        {
            WriteAuditEntries(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not null)
        {
            await WriteAuditEntriesAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void WriteAuditEntries(
        DbContext dbContext
    )
    {
        var auditLogs = CreateAuditLogs(dbContext);
        if (auditLogs.Count == 0)
        {
            return;
        }

        auditSink.Write(dbContext, auditLogs);
    }

    private async ValueTask WriteAuditEntriesAsync(
        DbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var auditLogs = CreateAuditLogs(dbContext);
        if (auditLogs.Count == 0)
        {
            return;
        }

        await auditSink.WriteAsync(dbContext, auditLogs, cancellationToken);
    }

    private IReadOnlyList<AuditLog> CreateAuditLogs(
        DbContext dbContext
    )
    {
        var currentOptions = options.Value;
        if (!currentOptions.Enabled)
        {
            return [];
        }

        dbContext.ChangeTracker.DetectChanges();

        var contextSnapshot = auditContextAccessor.GetCurrent();
        var auditLogs = dbContext.ChangeTracker.Entries()
            .Where(entry => ShouldAudit(entry, currentOptions))
            .Select(entry => CreateAuditLog(entry, contextSnapshot, currentOptions))
            .Where(static auditLog => auditLog is not null)
            .OfType<AuditLog>()
            .ToList();

        if (currentOptions.EnableHashChain)
        {
            AuditHashService.ApplyHashChain(dbContext, auditLogs);
        }

        return auditLogs;
    }

    private AuditLog? CreateAuditLog(
        EntityEntry entry,
        AuditContextSnapshot contextSnapshot,
        AuditLoggingOptions currentOptions
    )
    {
        var changes = CreateChangeSet(entry, currentOptions);
        if (changes.Count == 0)
        {
            return null;
        }

        return new AuditLog
        {
            EntityName = entry.Metadata.ClrType.Name,
            EntityId = entityIdResolver.Resolve(entry),
            Operation = ResolveOperation(entry),
            UserId = contextSnapshot.UserId,
            UserName = contextSnapshot.UserName,
            TenantId = contextSnapshot.TenantId,
            IpAddress = contextSnapshot.IpAddress,
            TraceId = contextSnapshot.TraceId,
            Source = contextSnapshot.Source,
            TimestampUtc = DateTime.UtcNow,
            ChangesJson = JsonSerializer.Serialize(changes, AuditJsonSerializer.Options),
            MetadataJson = "{}"
        };
    }

    private static SortedDictionary<string, AuditPropertyChange> CreateChangeSet(
        EntityEntry entry,
        AuditLoggingOptions currentOptions
    )
    {
        var changes = new SortedDictionary<string, AuditPropertyChange>(StringComparer.Ordinal);

        foreach (var property in entry.Properties.Where(property => ShouldAuditProperty(property, currentOptions)))
        {
            var propertyName = property.Metadata.Name;
            var isRedacted = IsRedacted(property);

            if (entry.State == EntityState.Added)
            {
                changes[propertyName] = CreateChange(null, property.CurrentValue, isRedacted, currentOptions);
                continue;
            }

            if (entry.State == EntityState.Deleted)
            {
                changes[propertyName] = CreateChange(property.OriginalValue, null, isRedacted, currentOptions);
                continue;
            }

            if (ShouldIncludeModifiedProperty(entry, property, currentOptions))
            {
                changes[propertyName] = CreateChange(
                    property.OriginalValue,
                    property.CurrentValue,
                    isRedacted,
                    currentOptions
                );
            }
        }

        return changes;
    }

    private static AuditPropertyChange CreateChange(
        object? oldValue,
        object? newValue,
        bool isRedacted,
        AuditLoggingOptions currentOptions
    )
    {
        if (isRedacted)
        {
            return new AuditPropertyChange
            {
                Old = currentOptions.RedactedValue,
                New = currentOptions.RedactedValue
            };
        }

        return new AuditPropertyChange
        {
            Old = oldValue,
            New = newValue
        };
    }

    private static bool ShouldIncludeModifiedProperty(
        EntityEntry entry,
        PropertyEntry property,
        AuditLoggingOptions currentOptions
    )
    {
        if (entry.State == EntityState.Unchanged)
        {
            return currentOptions.IncludeUnchangedOwnedTypes && entry.Metadata.IsOwned();
        }

        return property.IsModified && ValuesDiffer(property.OriginalValue, property.CurrentValue);
    }

    private static bool ShouldAudit(
        EntityEntry entry,
        AuditLoggingOptions currentOptions
    )
    {
        if (entry.Entity is AuditLog or AuditOutbox)
        {
            return false;
        }

        if (HasAuditIgnore(entry.Metadata.ClrType) || HasAuditIgnore(entry.Entity.GetType()))
        {
            return false;
        }

        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
            || (
                currentOptions.IncludeUnchangedOwnedTypes
                && entry.State == EntityState.Unchanged
                && entry.Metadata.IsOwned()
            );
    }

    private static bool ShouldAuditProperty(
        PropertyEntry property,
        AuditLoggingOptions currentOptions
    )
    {
        if (!currentOptions.IncludeShadowProperties && property.Metadata.IsShadowProperty())
        {
            return false;
        }

        return !HasAttribute<AuditIgnoreAttribute>(property.Metadata);
    }

    private static AuditOperation ResolveOperation(
        EntityEntry entry
    )
    {
        return entry.State switch
        {
            EntityState.Added => AuditOperation.Insert,
            EntityState.Deleted => AuditOperation.Delete,
            EntityState.Modified when IsSoftDelete(entry) => AuditOperation.SoftDelete,
            _ => AuditOperation.Update
        };
    }

    private static bool IsSoftDelete(
        EntityEntry entry
    )
    {
        var isDeleted = entry.Properties.FirstOrDefault(
            static property => property.Metadata.Name == "IsDeleted"
        );

        return isDeleted is
        {
            IsModified: true,
            OriginalValue: false,
            CurrentValue: true
        };
    }

    private static bool ValuesDiffer(
        object? oldValue,
        object? newValue
    )
    {
        return oldValue is null
            ? newValue is not null
            : !oldValue.Equals(newValue);
    }

    private static bool IsRedacted(
        PropertyEntry property
    )
    {
        return HasAttribute<AuditRedactAttribute>(property.Metadata);
    }

    private static bool HasAuditIgnore(
        Type type
    )
    {
        return type.GetCustomAttribute<AuditIgnoreAttribute>(inherit: true) is not null;
    }

    private static bool HasAttribute<TAttribute>(
        IReadOnlyProperty property
    ) where TAttribute : Attribute
    {
        return HasAttribute<TAttribute>(property.PropertyInfo)
            || HasAttribute<TAttribute>(property.FieldInfo);
    }

    private static bool HasAttribute<TAttribute>(
        MemberInfo? memberInfo
    ) where TAttribute : Attribute
    {
        return memberInfo?.GetCustomAttribute<TAttribute>(inherit: true) is not null;
    }
}
