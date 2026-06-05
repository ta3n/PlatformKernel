using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Abstractions;
using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Services;

public sealed class EfCoreAuditOutboxSink : IAuditSink
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

        var outboxMessages = auditLogs.Select(
            auditLog => new AuditOutbox
            {
                PayloadJson = JsonSerializer.Serialize(auditLog, AuditJsonSerializer.Options),
                CreatedUtc = DateTime.UtcNow
            }
        );

        dbContext.Set<AuditOutbox>().AddRange(outboxMessages);
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
