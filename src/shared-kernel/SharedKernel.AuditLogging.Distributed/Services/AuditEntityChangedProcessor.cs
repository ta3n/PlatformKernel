using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel.AuditLogging.Distributed.Contracts;
using SharedKernel.AuditLogging.Distributed.Models;
using SharedKernel.AuditLogging.Distributed.Options;
using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Distributed.Services;

public sealed class AuditEntityChangedProcessor(
    IOptions<AuditServiceOptions> options
)
{
    public async Task<AuditProcessingResult> ProcessAsync(
        DbContext dbContext,
        AuditEntityChanged auditEvent,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(auditEvent);

        Validate(auditEvent);

        if (await IsProcessedAsync(dbContext, auditEvent.EventId, cancellationToken))
        {
            return AuditProcessingResult.Duplicate();
        }

        var state = await GetStreamStateAsync(dbContext, auditEvent, cancellationToken);
        if (RequiresPending(auditEvent, state, options.Value))
        {
            await StorePendingIfMissingAsync(dbContext, auditEvent, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return AuditProcessingResult.Pending();
        }

        var appendedLogs = await AppendAndDrainAsync(dbContext, auditEvent, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return appendedLogs == 0
            ? AuditProcessingResult.Skipped()
            : AuditProcessingResult.Processed(appendedLogs);
    }

    private async Task<int> AppendAndDrainAsync(
        DbContext dbContext,
        AuditEntityChanged auditEvent,
        CancellationToken cancellationToken
    )
    {
        var appendedLogs = await AppendIfExpectedAsync(dbContext, auditEvent, cancellationToken);
        if (!HasStrictVersion(auditEvent))
        {
            return appendedLogs;
        }

        var lastProcessedVersion = auditEvent.EntityVersion.GetValueOrDefault();
        while (true)
        {
            var nextVersion = lastProcessedVersion + 1;
            var pending = await dbContext.Set<AuditPendingEvent>()
                .Where(message =>
                    message.SourceService == auditEvent.SourceService
                    && message.EntityName == auditEvent.EntityName
                    && message.EntityKey == auditEvent.EntityKey
                    && message.EntityVersion == nextVersion
                )
                .OrderBy(static message => message.ReceivedUtc)
                .FirstOrDefaultAsync(cancellationToken);

            if (pending is null)
            {
                return appendedLogs;
            }

            var pendingEvent = JsonSerializer.Deserialize<AuditEntityChanged>(
                pending.PayloadJson,
                AuditDistributedJsonSerializer.Options
            ) ?? throw new InvalidOperationException("Pending audit event payload is empty.");

            dbContext.Set<AuditPendingEvent>().Remove(pending);
            appendedLogs += await AppendIfExpectedAsync(dbContext, pendingEvent, cancellationToken);
            lastProcessedVersion = pendingEvent.EntityVersion.GetValueOrDefault();
        }
    }

    private async Task<int> AppendIfExpectedAsync(
        DbContext dbContext,
        AuditEntityChanged auditEvent,
        CancellationToken cancellationToken
    )
    {
        if (await IsProcessedAsync(dbContext, auditEvent.EventId, cancellationToken))
        {
            return 0;
        }

        var state = await GetStreamStateAsync(dbContext, auditEvent, cancellationToken);
        if (HasStrictVersion(auditEvent) && IsStale(auditEvent, state))
        {
            AddProcessedEvent(dbContext, auditEvent);
            return 0;
        }

        var changesJson = AuditEventDiffService.CreateChangesJson(auditEvent);
        var appendLog = !options.Value.SkipEmptyChanges || changesJson != "{}";
        if (appendLog)
        {
            dbContext.Set<AuditLog>().Add(CreateAuditLog(auditEvent, changesJson));
        }

        AddProcessedEvent(dbContext, auditEvent);
        UpsertStreamState(dbContext, auditEvent, state);

        return appendLog ? 1 : 0;
    }

    private static AuditLog CreateAuditLog(
        AuditEntityChanged auditEvent,
        string changesJson
    )
    {
        var metadata = new
        {
            auditEvent.EventId,
            auditEvent.EntityVersion,
            auditEvent.SchemaVersion,
            auditEvent.MetadataJson
        };

        return new AuditLog
        {
            EntityName = auditEvent.EntityName,
            EntityId = auditEvent.EntityKey,
            Operation = auditEvent.Operation,
            UserId = auditEvent.UserId,
            UserName = auditEvent.UserName,
            TenantId = auditEvent.TenantId,
            IpAddress = auditEvent.IpAddress,
            TraceId = auditEvent.TraceId,
            Source = auditEvent.SourceService,
            TimestampUtc = auditEvent.OccurredUtc.UtcDateTime,
            ChangesJson = changesJson,
            MetadataJson = JsonSerializer.Serialize(metadata, AuditDistributedJsonSerializer.Options)
        };
    }

    private static void AddProcessedEvent(
        DbContext dbContext,
        AuditEntityChanged auditEvent
    )
    {
        dbContext.Set<AuditProcessedEvent>().Add(
            new AuditProcessedEvent
            {
                EventId = auditEvent.EventId,
                SourceService = auditEvent.SourceService,
                EntityName = auditEvent.EntityName,
                EntityKey = auditEvent.EntityKey,
                EntityVersion = auditEvent.EntityVersion,
                ProcessedUtc = DateTimeOffset.UtcNow
            }
        );
    }

    private static void UpsertStreamState(
        DbContext dbContext,
        AuditEntityChanged auditEvent,
        AuditEntityStreamState? state
    )
    {
        if (!HasStrictVersion(auditEvent))
        {
            return;
        }

        if (state is null)
        {
            dbContext.Set<AuditEntityStreamState>().Add(
                new AuditEntityStreamState
                {
                    SourceService = auditEvent.SourceService,
                    EntityName = auditEvent.EntityName,
                    EntityKey = auditEvent.EntityKey,
                    LastProcessedVersion = auditEvent.EntityVersion.GetValueOrDefault(),
                    UpdatedUtc = DateTimeOffset.UtcNow
                }
            );
            return;
        }

        state.LastProcessedVersion = auditEvent.EntityVersion.GetValueOrDefault();
        state.UpdatedUtc = DateTimeOffset.UtcNow;
    }

    private async Task StorePendingIfMissingAsync(
        DbContext dbContext,
        AuditEntityChanged auditEvent,
        CancellationToken cancellationToken
    )
    {
        var exists = await dbContext.Set<AuditPendingEvent>()
            .AnyAsync(message => message.EventId == auditEvent.EventId, cancellationToken);
        if (exists)
        {
            return;
        }

        dbContext.Set<AuditPendingEvent>().Add(
            new AuditPendingEvent
            {
                EventId = auditEvent.EventId,
                SourceService = auditEvent.SourceService,
                EntityName = auditEvent.EntityName,
                EntityKey = auditEvent.EntityKey,
                EntityVersion = auditEvent.EntityVersion.GetValueOrDefault(),
                PayloadJson = JsonSerializer.Serialize(auditEvent, AuditDistributedJsonSerializer.Options),
                ReceivedUtc = DateTimeOffset.UtcNow
            }
        );
    }

    private static async Task<bool> IsProcessedAsync(
        DbContext dbContext,
        Guid eventId,
        CancellationToken cancellationToken
    )
    {
        return await dbContext.Set<AuditProcessedEvent>()
            .AnyAsync(auditEvent => auditEvent.EventId == eventId, cancellationToken);
    }

    private static async Task<AuditEntityStreamState?> GetStreamStateAsync(
        DbContext dbContext,
        AuditEntityChanged auditEvent,
        CancellationToken cancellationToken
    )
    {
        var localState = dbContext.Set<AuditEntityStreamState>()
            .Local
            .SingleOrDefault(state =>
                state.SourceService == auditEvent.SourceService
                && state.EntityName == auditEvent.EntityName
                && state.EntityKey == auditEvent.EntityKey
            );
        if (localState is not null)
        {
            return localState;
        }

        return await dbContext.Set<AuditEntityStreamState>()
            .SingleOrDefaultAsync(
                state =>
                    state.SourceService == auditEvent.SourceService
                    && state.EntityName == auditEvent.EntityName
                    && state.EntityKey == auditEvent.EntityKey,
                cancellationToken
            );
    }

    private static bool RequiresPending(
        AuditEntityChanged auditEvent,
        AuditEntityStreamState? state,
        AuditServiceOptions options
    )
    {
        if (!HasStrictVersion(auditEvent))
        {
            if (options.RequireEntityVersion)
            {
                throw new InvalidOperationException("Audit event does not contain an entity version.");
            }

            return false;
        }

        var entityVersion = auditEvent.EntityVersion.GetValueOrDefault();
        if (state is null)
        {
            return !options.AllowFirstVersionToStartAtAnyValue && entityVersion > 1;
        }

        var expectedVersion = state.LastProcessedVersion + 1;
        return entityVersion > expectedVersion;
    }

    private static bool IsStale(
        AuditEntityChanged auditEvent,
        AuditEntityStreamState? state
    )
    {
        return state is not null
            && auditEvent.EntityVersion.GetValueOrDefault() <= state.LastProcessedVersion;
    }

    private static bool HasStrictVersion(
        AuditEntityChanged auditEvent
    )
    {
        return auditEvent.EntityVersion is > 0;
    }

    private static void Validate(
        AuditEntityChanged auditEvent
    )
    {
        if (auditEvent.EventId == Guid.Empty)
        {
            throw new InvalidOperationException("Audit event id is required.");
        }

        if (string.IsNullOrWhiteSpace(auditEvent.SourceService))
        {
            throw new InvalidOperationException("Audit event source service is required.");
        }

        if (string.IsNullOrWhiteSpace(auditEvent.EntityName))
        {
            throw new InvalidOperationException("Audit event entity name is required.");
        }

        if (string.IsNullOrWhiteSpace(auditEvent.EntityKey))
        {
            throw new InvalidOperationException("Audit event entity key is required.");
        }
    }
}
