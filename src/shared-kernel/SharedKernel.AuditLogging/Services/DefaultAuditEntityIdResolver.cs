using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.Entity;

namespace SharedKernel.AuditLogging.Services;

public sealed class DefaultAuditEntityIdResolver : IAuditEntityIdResolver
{
    public string Resolve(
        EntityEntry entry
    )
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.Entity is EntityData entityData && !string.IsNullOrWhiteSpace(entityData.Code))
        {
            return entityData.Code;
        }

        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey is null)
        {
            return string.Empty;
        }

        var values = primaryKey.Properties
            .Select(property => ResolvePropertyValue(entry.Property(property.Name)))
            .Where(static value => !string.IsNullOrWhiteSpace(value));

        return string.Join("|", values);
    }

    private static string? ResolvePropertyValue(
        PropertyEntry propertyEntry
    )
    {
        var value = propertyEntry.CurrentValue ?? propertyEntry.OriginalValue;
        return value?.ToString();
    }
}
