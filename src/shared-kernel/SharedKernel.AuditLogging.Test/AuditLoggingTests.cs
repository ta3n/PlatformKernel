using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.AuditLogging.Attributes;
using SharedKernel.AuditLogging.Distributed.Contracts;
using SharedKernel.AuditLogging.Distributed.Models;
using SharedKernel.AuditLogging.Distributed.Options;
using SharedKernel.AuditLogging.Distributed.Services;
using SharedKernel.AuditLogging.Extensions;
using SharedKernel.AuditLogging.Models;
using SharedKernel.AuditLogging.Options;
using SharedKernel.AuditLogging.Services;

namespace SharedKernel.AuditLogging.Test;

public sealed class AuditLoggingTests
{
    [Fact]
    public async Task SaveChanges_CapturesInsertUpdateSoftDeleteAndDelete()
    {
        using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestAuditDbContext>();
        var entity = new TestEntity { Name = "first" };

        dbContext.Entities.Add(entity);
        await dbContext.SaveChangesAsync();

        entity.Name = "second";
        await dbContext.SaveChangesAsync();

        entity.IsDeleted = true;
        await dbContext.SaveChangesAsync();

        dbContext.Entities.Remove(entity);
        await dbContext.SaveChangesAsync();

        var auditLogs = await dbContext.AuditLogs
            .OrderBy(static auditLog => auditLog.TimestampUtc)
            .ToListAsync();

        Assert.Equal(
            [
                AuditOperation.Insert,
                AuditOperation.Update,
                AuditOperation.SoftDelete,
                AuditOperation.Delete
            ],
            auditLogs.Select(static auditLog => auditLog.Operation)
        );
        Assert.Contains("\"Name\"", auditLogs[1].ChangesJson, StringComparison.Ordinal);
        Assert.Contains("\"old\":\"first\"", auditLogs[1].ChangesJson, StringComparison.Ordinal);
        Assert.Contains("\"new\":\"second\"", auditLogs[1].ChangesJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SaveChanges_RespectsIgnoreAndRedactAttributes()
    {
        using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestAuditDbContext>();

        dbContext.RedactedEntities.Add(
            new RedactedEntity
            {
                Name = "visible",
                Secret = "sensitive-value",
                InternalNote = "internal-value"
            }
        );
        dbContext.IgnoredEntities.Add(new IgnoredEntity { Name = "ignored" });

        await dbContext.SaveChangesAsync();

        var auditLog = await dbContext.AuditLogs.SingleAsync();

        Assert.Equal(nameof(RedactedEntity), auditLog.EntityName);
        Assert.Contains("\"Secret\"", auditLog.ChangesJson, StringComparison.Ordinal);
        Assert.Contains("***REDACTED***", auditLog.ChangesJson, StringComparison.Ordinal);
        Assert.DoesNotContain("sensitive-value", auditLog.ChangesJson, StringComparison.Ordinal);
        Assert.DoesNotContain("InternalNote", auditLog.ChangesJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SaveChanges_UsesHttpContextWhenAvailable()
    {
        using var provider = CreateProvider(options => options.Source = "unit-test");
        using var scope = provider.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        accessor.HttpContext = CreateHttpContext();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestAuditDbContext>();

        dbContext.Entities.Add(new TestEntity { Name = "context" });
        await dbContext.SaveChangesAsync();

        var auditLog = await dbContext.AuditLogs.SingleAsync();

        Assert.Equal("user-1", auditLog.UserId);
        Assert.Equal("Alice", auditLog.UserName);
        Assert.Equal("tenant-1", auditLog.TenantId);
        Assert.Equal("127.0.0.1", auditLog.IpAddress);
        Assert.Equal("unit-test", auditLog.Source);
        Assert.False(string.IsNullOrWhiteSpace(auditLog.TraceId));
    }

    [Fact]
    public async Task SaveChanges_FallsBackToSystemWithoutHttpContext()
    {
        using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestAuditDbContext>();

        dbContext.Entities.Add(new TestEntity { Name = "background" });
        await dbContext.SaveChangesAsync();

        var auditLog = await dbContext.AuditLogs.SingleAsync();

        Assert.Equal("SYSTEM", auditLog.UserId);
        Assert.Equal("SYSTEM", auditLog.UserName);
    }

    [Fact]
    public async Task OutboxMode_WritesOutboxAndProcessorMovesPayloadToAuditLog()
    {
        using var provider = CreateProvider(options => options.Mode = AuditLoggingMode.Outbox);
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestAuditDbContext>();

        dbContext.Entities.Add(new TestEntity { Name = "outbox" });
        await dbContext.SaveChangesAsync();

        Assert.Empty(dbContext.AuditLogs);
        var outboxMessage = await dbContext.AuditOutbox.SingleAsync();
        Assert.Null(outboxMessage.ProcessedUtc);

        var processor = scope.ServiceProvider.GetRequiredService<AuditOutboxProcessor>();
        var processed = await processor.ProcessBatchAsync(dbContext);

        Assert.Equal(1, processed);
        Assert.Equal(1, await dbContext.AuditLogs.CountAsync());
        Assert.NotNull(outboxMessage.ProcessedUtc);
    }

    [Fact]
    public async Task SaveChanges_DoesNotAuditAuditLogOrAuditOutboxEntities()
    {
        using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestAuditDbContext>();

        dbContext.AuditLogs.Add(
            new AuditLog
            {
                EntityName = "Manual",
                EntityId = "1",
                Operation = AuditOperation.Insert,
                UserId = "SYSTEM",
                UserName = "SYSTEM",
                Source = "test"
            }
        );
        dbContext.AuditOutbox.Add(new AuditOutbox { PayloadJson = "{}" });

        await dbContext.SaveChangesAsync();

        Assert.Equal(1, await dbContext.AuditLogs.CountAsync());
        Assert.Equal(1, await dbContext.AuditOutbox.CountAsync());
    }

    [Fact]
    public async Task SaveChanges_AddsPreviousHashWhenHashChainIsEnabled()
    {
        using var provider = CreateProvider(options => options.EnableHashChain = true);
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestAuditDbContext>();
        var entity = new TestEntity { Name = "first" };

        dbContext.Entities.Add(entity);
        await dbContext.SaveChangesAsync();

        entity.Name = "second";
        await dbContext.SaveChangesAsync();

        var auditLogs = await dbContext.AuditLogs
            .OrderBy(static auditLog => auditLog.TimestampUtc)
            .ToListAsync();

        Assert.Equal(2, auditLogs.Count);
        Assert.Null(auditLogs[0].PreviousHash);
        Assert.False(string.IsNullOrWhiteSpace(auditLogs[0].Hash));
        Assert.Equal(auditLogs[0].Hash, auditLogs[1].PreviousHash);
        Assert.False(string.IsNullOrWhiteSpace(auditLogs[1].Hash));
    }

    [Fact]
    public async Task DistributedAuditService_ComputesDiffFromSnapshots()
    {
        await using var dbContext = CreateDistributedDbContext();
        var processor = CreateDistributedProcessor();
        var auditEvent = CreateAuditEvent(
            version: 1,
            beforeJson: """{"Status":"Pending","Total":100}""",
            afterJson: """{"Status":"Confirmed","Total":100}"""
        );

        var result = await processor.ProcessAsync(dbContext, auditEvent);

        var auditLog = await dbContext.AuditLogs.SingleAsync();
        Assert.Equal(AuditProcessingStatus.Processed, result.Status);
        Assert.Equal(1, result.AppendedLogs);
        Assert.Contains("\"Status\"", auditLog.ChangesJson, StringComparison.Ordinal);
        Assert.Contains("\"old\":\"Pending\"", auditLog.ChangesJson, StringComparison.Ordinal);
        Assert.Contains("\"new\":\"Confirmed\"", auditLog.ChangesJson, StringComparison.Ordinal);
        Assert.DoesNotContain("\"Total\"", auditLog.ChangesJson, StringComparison.Ordinal);
        Assert.Equal(1, await dbContext.ProcessedEvents.CountAsync());
        Assert.Equal(1, (await dbContext.StreamStates.SingleAsync()).LastProcessedVersion);
    }

    [Fact]
    public async Task DistributedAuditService_HoldsOutOfOrderEventAndDrainsWhenGapArrives()
    {
        await using var dbContext = CreateDistributedDbContext();
        var processor = CreateDistributedProcessor();
        var versionTwo = CreateAuditEvent(
            version: 2,
            beforeJson: """{"Status":"Confirmed"}""",
            afterJson: """{"Status":"Cancelled"}"""
        );
        var versionOne = CreateAuditEvent(
            version: 1,
            beforeJson: """{"Status":"Pending"}""",
            afterJson: """{"Status":"Confirmed"}"""
        );

        var pendingResult = await processor.ProcessAsync(dbContext, versionTwo);

        Assert.Equal(AuditProcessingStatus.Pending, pendingResult.Status);
        Assert.Equal(0, await dbContext.AuditLogs.CountAsync());
        Assert.Equal(1, await dbContext.PendingEvents.CountAsync());

        var processedResult = await processor.ProcessAsync(dbContext, versionOne);

        Assert.Equal(AuditProcessingStatus.Processed, processedResult.Status);
        Assert.Equal(2, processedResult.AppendedLogs);
        Assert.Equal(2, await dbContext.AuditLogs.CountAsync());
        Assert.Equal(0, await dbContext.PendingEvents.CountAsync());
        Assert.Equal(2, (await dbContext.StreamStates.SingleAsync()).LastProcessedVersion);
    }

    [Fact]
    public async Task DistributedAuditService_IgnoresDuplicateEventId()
    {
        await using var dbContext = CreateDistributedDbContext();
        var processor = CreateDistributedProcessor();
        var eventId = Guid.NewGuid();
        var auditEvent = CreateAuditEvent(
            version: 1,
            beforeJson: """{"Status":"Pending"}""",
            afterJson: """{"Status":"Confirmed"}""",
            eventId: eventId
        );

        await processor.ProcessAsync(dbContext, auditEvent);
        var result = await processor.ProcessAsync(dbContext, auditEvent);

        Assert.Equal(AuditProcessingStatus.Duplicate, result.Status);
        Assert.Equal(1, await dbContext.AuditLogs.CountAsync());
        Assert.Equal(1, await dbContext.ProcessedEvents.CountAsync());
    }

    private static ServiceProvider CreateProvider(
        Action<AuditLoggingOptions>? configure = null
    )
    {
        var services = new ServiceCollection();
        services.AddAuditLogging(
            options =>
            {
                configure?.Invoke(options);
            }
        );
        services.AddDbContext<TestAuditDbContext>(
            (serviceProvider, optionsBuilder) =>
            {
                optionsBuilder
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .AddAuditLoggingInterceptor(serviceProvider);
            }
        );

        return services.BuildServiceProvider();
    }

    private static AuditEntityChangedProcessor CreateDistributedProcessor()
    {
        return new AuditEntityChangedProcessor(
            Microsoft.Extensions.Options.Options.Create(new AuditServiceOptions())
        );
    }

    private static DistributedAuditDbContext CreateDistributedDbContext()
    {
        var options = new DbContextOptionsBuilder<DistributedAuditDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DistributedAuditDbContext(options);
    }

    private static AuditEntityChanged CreateAuditEvent(
        long version,
        string beforeJson,
        string afterJson,
        Guid? eventId = null
    )
    {
        return new AuditEntityChanged
        {
            EventId = eventId ?? Guid.NewGuid(),
            SourceService = "booking-service",
            EntityName = "Booking",
            EntityKey = "booking-1",
            EntityVersion = version,
            Operation = AuditOperation.Update,
            OccurredUtc = DateTimeOffset.UtcNow,
            UserId = "user-1",
            UserName = "Alice",
            BeforeJson = beforeJson,
            AfterJson = afterJson
        };
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext
        {
            TraceIdentifier = "trace-1",
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "user-1"),
                        new Claim(ClaimTypes.Name, "Alice"),
                        new Claim("tenant_id", "tenant-1")
                    ],
                    "Test"
                )
            )
        };

        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        return httpContext;
    }

    private sealed class TestAuditDbContext(
        DbContextOptions<TestAuditDbContext> options
    ) : DbContext(options)
    {
        public DbSet<TestEntity> Entities => Set<TestEntity>();

        public DbSet<RedactedEntity> RedactedEntities => Set<RedactedEntity>();

        public DbSet<IgnoredEntity> IgnoredEntities => Set<IgnoredEntity>();

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<AuditOutbox> AuditOutbox => Set<AuditOutbox>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder
        )
        {
            modelBuilder.ApplyAuditLogging();
        }
    }

    private sealed class DistributedAuditDbContext(
        DbContextOptions<DistributedAuditDbContext> options
    ) : DbContext(options)
    {
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<AuditEntityStreamState> StreamStates => Set<AuditEntityStreamState>();

        public DbSet<AuditPendingEvent> PendingEvents => Set<AuditPendingEvent>();

        public DbSet<AuditProcessedEvent> ProcessedEvents => Set<AuditProcessedEvent>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder
        )
        {
            SharedKernel.AuditLogging.Extensions.ModelBuilderExtensions.ApplyAuditLogging(modelBuilder);
            SharedKernel.AuditLogging.Distributed.Extensions.ModelBuilderExtensions.ApplyDistributedAuditService(modelBuilder);
        }
    }

    private sealed class TestEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }

    private sealed class RedactedEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [AuditRedact]
        public string Secret { get; set; } = string.Empty;

        [AuditIgnore]
        public string InternalNote { get; set; } = string.Empty;
    }

    [AuditIgnore]
    private sealed class IgnoredEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
