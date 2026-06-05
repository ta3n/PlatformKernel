using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Abstractions;

public interface IAuditContextAccessor
{
    AuditContextSnapshot GetCurrent();
}
