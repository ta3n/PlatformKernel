using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SharedKernel.AuditLogging.Abstractions;

public interface IAuditEntityIdResolver
{
    string Resolve(EntityEntry entry);
}
