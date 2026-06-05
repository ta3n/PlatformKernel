using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.AuditLogging.Attributes;
using SharedKernel.AuditLogging.Distributed.Abstractions;
using SharedKernel.AuditLogging.Distributed.Contracts;
using SharedKernel.AuditLogging.Distributed.Models;
using SharedKernel.AuditLogging.Distributed.Options;
using SharedKernel.AuditLogging.Distributed.Services;
using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Distributed.Interceptors;

public sealed class AuditEntityChangedInterceptor(
    IOptions<AuditProducerOptions> options,
    IAuditContextAccessor auditContextAccessor,
    IAuditEntityIdResolver entityIdResolver,
    IAuditEntityVersionResolver entityVersionResolver,
    IPublishEndpoint publishEndpoint
) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not null)
        {
            PublishAuditEventsAsync(eventData.Context, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
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
            await PublishAuditEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishAuditEventsAsync(
        DbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var currentOptions = options.Value;
        if (!currentOptions.Enabled)
        {
            return;
        }

        var auditEvents = CreateAuditEvents(dbContext, currentOptions);
        foreach (var auditEvent in auditEvents)
        {
            await publishEndpoint.Publish(auditEvent, cancellationToken);
        }
    }

    private IReadOnlyList<AuditEntityChanged> CreateAuditEvents(
        DbContext dbContext,
        AuditProducerOptions currentOptions
    )
    {
        dbContext.ChangeTracker.DetectChanges();

        var contextSnapshot = auditContextAccessor.GetCurrent();
        return dbContext.ChangeTracker.Entries()
            .Where(entry => ShouldAudit(entry, currentOptions))
            .Select(entry => CreateAuditEvent(entry, contextSnapshot, currentOptions))
            .ToList();
    }

    private AuditEntityChanged CreateAuditEvent(
        EntityEntry entry,
        AuditContextSnapshot contextSnapshot,
        AuditProducerOptions currentOptions
    )
    {
        if (HasTemporaryKey(entry) && currentOptions.RequireStableEntityKey)
        {
            throw new InvalidOperationException(
                $"Audited entity '{entry.Metadata.ClrType.Name}' has a temporary key. "
                + "Use application-generated keys or disable RequireStableEntityKey."
            );
        }

        var entityKey = entityIdResolver.Resolve(entry);
        if (string.IsNullOrWhiteSpace(entityKey) && currentOptions.RequireStableEntityKey)
        {
            throw new InvalidOperationException(
                $"Audited entity '{entry.Metadata.ClrType.Name}' does not expose a stable entity key."
            );
        }

        var entityVersion = entityVersionResolver.Resolve(entry);
        if (entityVersion is null && currentOptions.RequireEntityVersion)
        {
            throw new InvalidOperationException(
                $"Audited entity '{entry.Metadata.ClrType.Name}' does not expose an audit entity version."
            );
        }

        return new AuditEntityChanged
        {
            SourceService = string.IsNullOrWhiteSpace(currentOptions.SourceService)
                ? contextSnapshot.Source
                : currentOptions.SourceService,
            EntityName = entry.Metadata.ClrType.Name,
            EntityKey = entityKey,
            EntityVersion = entityVersion,
            Operation = ResolveOperation(entry),
            OccurredUtc = DateTimeOffset.UtcNow,
            UserId = contextSnapshot.UserId,
            UserName = contextSnapshot.UserName,
            TenantId = contextSnapshot.TenantId,
            IpAddress = contextSnapshot.IpAddress,
            TraceId = contextSnapshot.TraceId,
            BeforeJson = CreateSnapshotJson(entry, useOriginalValues: true, currentOptions),
            AfterJson = CreateSnapshotJson(entry, useOriginalValues: false, currentOptions),
            SchemaVersion = currentOptions.SchemaVersion
        };
    }

    private static string CreateSnapshotJson(
        EntityEntry entry,
        bool useOriginalValues,
        AuditProducerOptions currentOptions
    )
    {
        if (entry.State == EntityState.Added && useOriginalValues)
        {
            return "{}";
        }

        if (entry.State == EntityState.Deleted && !useOriginalValues)
        {
            return "{}";
        }

        var snapshot = new JsonObject();
        foreach (var property in entry.Properties.Where(property => ShouldAuditProperty(property, currentOptions)))
        {
            var isRedacted = HasAttribute<AuditRedactAttribute>(property.Metadata);
            var value = useOriginalValues
                ? property.OriginalValue
                : property.CurrentValue;

            snapshot[property.Metadata.Name] = isRedacted
                ? JsonValue.Create(currentOptions.RedactedValue)
                : SerializeValue(value);
        }

        return snapshot.ToJsonString(AuditDistributedJsonSerializer.Options);
    }

    private static JsonNode? SerializeValue(
        object? value
    )
    {
        if (value is null)
        {
            return null;
        }

        return JsonSerializer.SerializeToNode(value, AuditDistributedJsonSerializer.Options);
    }

    private static bool ShouldAudit(
        EntityEntry entry,
        AuditProducerOptions currentOptions
    )
    {
        if (entry.Entity is AuditLog or AuditOutbox or AuditEntityStreamState or AuditPendingEvent or AuditProcessedEvent)
        {
            return false;
        }

        if (HasAuditIgnore(entry.Metadata.ClrType) || HasAuditIgnore(entry.Entity.GetType()))
        {
            return false;
        }

        return entry.State switch
        {
            EntityState.Added => HasAnyAuditableProperty(entry, currentOptions),
            EntityState.Modified => HasAnyChangedProperty(entry, currentOptions),
            EntityState.Deleted => HasAnyAuditableProperty(entry, currentOptions),
            _ => false
        };
    }

    private static bool HasAnyAuditableProperty(
        EntityEntry entry,
        AuditProducerOptions currentOptions
    )
    {
        return entry.Properties.Any(property => ShouldAuditProperty(property, currentOptions));
    }

    private static bool HasAnyChangedProperty(
        EntityEntry entry,
        AuditProducerOptions currentOptions
    )
    {
        return entry.Properties
            .Where(property => ShouldAuditProperty(property, currentOptions))
            .Any(static property => property.IsModified && ValuesDiffer(property.OriginalValue, property.CurrentValue));
    }

    private static bool ShouldAuditProperty(
        PropertyEntry property,
        AuditProducerOptions currentOptions
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

    private static bool HasTemporaryKey(
        EntityEntry entry
    )
    {
        return entry.Properties.Any(static property => property.Metadata.IsPrimaryKey() && property.IsTemporary);
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
