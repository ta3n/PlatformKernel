using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Abstractions;

public interface IAuditSink
{
    void Write(DbContext dbContext, IReadOnlyCollection<AuditLog> auditLogs);

    ValueTask WriteAsync(
        DbContext dbContext,
        IReadOnlyCollection<AuditLog> auditLogs,
        CancellationToken cancellationToken
    );
}
