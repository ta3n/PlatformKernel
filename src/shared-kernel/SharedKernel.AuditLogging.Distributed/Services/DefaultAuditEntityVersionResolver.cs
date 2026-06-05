using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel.AuditLogging.Distributed.Abstractions;

namespace SharedKernel.AuditLogging.Distributed.Services;

public sealed class DefaultAuditEntityVersionResolver : IAuditEntityVersionResolver
{
    private static readonly string[] VersionPropertyNames =
    [
        "AuditVersion",
        "EntityVersion",
        "Version"
    ];

    public long? Resolve(
        EntityEntry entry
    )
    {
        ArgumentNullException.ThrowIfNull(entry);

        foreach (var propertyName in VersionPropertyNames)
        {
            var property = entry.Properties.FirstOrDefault(
                candidate => string.Equals(
                    candidate.Metadata.Name,
                    propertyName,
                    StringComparison.Ordinal
                )
            );

            if (property is null)
            {
                continue;
            }

            var value = property.CurrentValue ?? property.OriginalValue;
            if (value is null)
            {
                continue;
            }

            if (TryConvertToInt64(value, out var version))
            {
                return version;
            }
        }

        return null;
    }

    private static bool TryConvertToInt64(
        object value,
        out long result
    )
    {
        try
        {
            result = Convert.ToInt64(value);
            return true;
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            result = 0;
            return false;
        }
    }
}
