using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Services;

public static class AuditHashService
{
    public static void ApplyHashChain(
        DbContext dbContext,
        IReadOnlyList<AuditLog> auditLogs
    )
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(auditLogs);

        if (auditLogs.Count == 0)
        {
            return;
        }

        var previousHash = GetPreviousHash(dbContext);
        foreach (var auditLog in auditLogs)
        {
            auditLog.PreviousHash = previousHash;
            auditLog.Hash = ComputeHash(auditLog);
            previousHash = auditLog.Hash;
        }
    }

    private static string? GetPreviousHash(
        DbContext dbContext
    )
    {
        return dbContext.Set<AuditLog>()
            .AsNoTracking()
            .Where(static auditLog => auditLog.Hash != null)
            .OrderByDescending(static auditLog => auditLog.TimestampUtc)
            .ThenByDescending(static auditLog => auditLog.Id)
            .Select(static auditLog => auditLog.Hash)
            .FirstOrDefault();
    }

    private static string ComputeHash(
        AuditLog auditLog
    )
    {
        var payload = new SortedDictionary<string, object?>
        {
            [nameof(AuditLog.ChangesJson)] = auditLog.ChangesJson,
            [nameof(AuditLog.EntityId)] = auditLog.EntityId,
            [nameof(AuditLog.EntityName)] = auditLog.EntityName,
            [nameof(AuditLog.Id)] = auditLog.Id,
            [nameof(AuditLog.IpAddress)] = auditLog.IpAddress,
            [nameof(AuditLog.MetadataJson)] = auditLog.MetadataJson,
            [nameof(AuditLog.Operation)] = auditLog.Operation,
            [nameof(AuditLog.Source)] = auditLog.Source,
            [nameof(AuditLog.TenantId)] = auditLog.TenantId,
            [nameof(AuditLog.TimestampUtc)] = auditLog.TimestampUtc,
            [nameof(AuditLog.TraceId)] = auditLog.TraceId,
            [nameof(AuditLog.UserId)] = auditLog.UserId,
            [nameof(AuditLog.UserName)] = auditLog.UserName
        };

        var canonicalPayload = JsonSerializer.Serialize(payload, AuditJsonSerializer.Options);
        var input = canonicalPayload + (auditLog.PreviousHash ?? string.Empty);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
