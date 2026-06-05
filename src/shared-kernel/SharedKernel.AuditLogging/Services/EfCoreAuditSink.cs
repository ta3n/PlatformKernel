using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Services;

public sealed class EfCoreAuditSink : IAuditSink
{
    public void Write(
        DbContext dbContext,
        IReadOnlyCollection<AuditLog> auditLogs
    )
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(auditLogs);

        if (auditLogs.Count == 0)
        {
            return;
        }

        dbContext.Set<AuditLog>().AddRange(auditLogs);
    }

    public ValueTask WriteAsync(
        DbContext dbContext,
        IReadOnlyCollection<AuditLog> auditLogs,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        Write(dbContext, auditLogs);
        return ValueTask.CompletedTask;
    }
}
