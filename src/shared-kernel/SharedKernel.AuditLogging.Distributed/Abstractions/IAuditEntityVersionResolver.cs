using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SharedKernel.AuditLogging.Distributed.Abstractions;

public interface IAuditEntityVersionResolver
{
    long? Resolve(EntityEntry entry);
}
