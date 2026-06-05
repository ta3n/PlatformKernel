using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel.AuditLogging.Models;
using SharedKernel.AuditLogging.Options;

namespace SharedKernel.AuditLogging.Services;

public sealed class AuditOutboxProcessor(
    IOptions<AuditLoggingOptions> options
)
{
    public async Task<int> ProcessBatchAsync(
        DbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var batchSize = Math.Max(1, options.Value.OutboxBatchSize);
        var messages = await dbContext.Set<AuditOutbox>()
            .Where(static message => message.ProcessedUtc == null)
            .OrderBy(static message => message.CreatedUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return 0;
        }

        var processed = 0;
        foreach (var message in messages)
        {
            try
            {
                var auditLog = JsonSerializer.Deserialize<AuditLog>(
                    message.PayloadJson,
                    AuditJsonSerializer.Options
                ) ?? throw new InvalidOperationException("Audit outbox payload is empty.");

                dbContext.Set<AuditLog>().Add(auditLog);
                message.ProcessedUtc = DateTime.UtcNow;
                message.LastError = null;
                processed++;
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException)
            {
                message.Attempts++;
                message.LastError = ex.Message;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return processed;
    }
}
