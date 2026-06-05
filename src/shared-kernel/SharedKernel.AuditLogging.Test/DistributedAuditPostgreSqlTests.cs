using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Distributed.Contracts;
using SharedKernel.AuditLogging.Distributed.Extensions;
using SharedKernel.AuditLogging.Distributed.Models;
using SharedKernel.AuditLogging.Distributed.Options;
using SharedKernel.AuditLogging.Distributed.Services;
using SharedKernel.AuditLogging.Models;
using Testcontainers.PostgreSql;

namespace SharedKernel.AuditLogging.Test;

public sealed class DistributedAuditPostgreSqlTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("audit_tests")
        .WithUsername("audit_user")
        .WithPassword("audit_password")
        .Build();

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();
        await using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ProcessAsync_WithPostgreSql_PersistsOutOfOrderEventsInStreamOrder()
    {
        await using (var dbContext = CreateDbContext())
        {
            var processor = CreateProcessor();

            var pending = await processor.ProcessAsync(
                dbContext,
                CreateAuditEvent(
                    version: 2,
                    beforeJson: """{"status":"confirmed"}""",
                    afterJson: """{"status":"cancelled"}"""
                )
            );

            Assert.Equal(AuditProcessingStatus.Pending, pending.Status);
        }

        await using (var dbContext = CreateDbContext())
        {
            Assert.Equal(1, await dbContext.PendingEvents.CountAsync());
            Assert.Equal(0, await dbContext.AuditLogs.CountAsync());

            var processor = CreateProcessor();
            var processed = await processor.ProcessAsync(
                dbContext,
                CreateAuditEvent(
                    version: 1,
                    beforeJson: """{"status":"pending"}""",
                    afterJson: """{"status":"confirmed"}"""
                )
            );

            Assert.Equal(AuditProcessingStatus.Processed, processed.Status);
            Assert.Equal(2, processed.AppendedLogs);
        }

        await using (var dbContext = CreateDbContext())
        {
            var logs = await dbContext.AuditLogs
                .OrderBy(static auditLog => auditLog.TimestampUtc)
                .ToListAsync();

            Assert.Equal(2, logs.Count);
            Assert.Contains("\"new\":\"confirmed\"", logs[0].ChangesJson, StringComparison.Ordinal);
            Assert.Contains("\"new\":\"cancelled\"", logs[1].ChangesJson, StringComparison.Ordinal);
            Assert.Empty(await dbContext.PendingEvents.ToListAsync());
            Assert.Equal(2, await dbContext.ProcessedEvents.CountAsync());
            Assert.Equal(2, (await dbContext.StreamStates.SingleAsync()).LastProcessedVersion);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ProcessAsync_WithPostgreSql_IgnoresDuplicateEventAcrossDbContexts()
    {
        var auditEvent = CreateAuditEvent(
            version: 1,
            beforeJson: """{"status":"pending"}""",
            afterJson: """{"status":"confirmed"}"""
        );

        await using (var dbContext = CreateDbContext())
        {
            var result = await CreateProcessor().ProcessAsync(dbContext, auditEvent);
            Assert.Equal(AuditProcessingStatus.Processed, result.Status);
        }

        await using (var dbContext = CreateDbContext())
        {
            var result = await CreateProcessor().ProcessAsync(dbContext, auditEvent);
            Assert.Equal(AuditProcessingStatus.Duplicate, result.Status);
        }

        await using (var dbContext = CreateDbContext())
        {
            Assert.Equal(1, await dbContext.AuditLogs.CountAsync());
            Assert.Equal(1, await dbContext.ProcessedEvents.CountAsync());
        }
    }

    private DistributedAuditPostgreSqlDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DistributedAuditPostgreSqlDbContext>()
            .UseNpgsql(postgreSqlContainer.GetConnectionString())
            .Options;

        return new DistributedAuditPostgreSqlDbContext(options);
    }

    private static AuditEntityChangedProcessor CreateProcessor()
    {
        return new AuditEntityChangedProcessor(
            Microsoft.Extensions.Options.Options.Create(new AuditServiceOptions())
        );
    }

    private static AuditEntityChanged CreateAuditEvent(
        long version,
        string beforeJson,
        string afterJson
    )
    {
        return new AuditEntityChanged
        {
            EventId = Guid.NewGuid(),
            SourceService = "booking-service",
            EntityName = "Booking",
            EntityKey = "booking-1",
            EntityVersion = version,
            Operation = AuditOperation.Update,
            OccurredUtc = DateTimeOffset.UtcNow.AddSeconds(version),
            UserId = "user-1",
            UserName = "Alice",
            BeforeJson = beforeJson,
            AfterJson = afterJson
        };
    }

    private sealed class DistributedAuditPostgreSqlDbContext(
        DbContextOptions<DistributedAuditPostgreSqlDbContext> options
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
            modelBuilder.ApplyDistributedAuditService();
        }
    }
}
