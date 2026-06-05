using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Distributed.Models;

namespace SharedKernel.AuditLogging.Distributed.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyDistributedAuditService(
        this ModelBuilder modelBuilder
    )
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<AuditEntityStreamState>(
            builder =>
            {
                builder.ToTable("AuditEntityStreamStates");
                builder.HasKey(static state => state.Id);
                builder.Property(static state => state.SourceService).HasMaxLength(256).IsRequired();
                builder.Property(static state => state.EntityName).HasMaxLength(256).IsRequired();
                builder.Property(static state => state.EntityKey).HasMaxLength(256).IsRequired();
                builder.HasIndex(static state => new { state.SourceService, state.EntityName, state.EntityKey })
                    .IsUnique();
            }
        );

        modelBuilder.Entity<AuditProcessedEvent>(
            builder =>
            {
                builder.ToTable("AuditProcessedEvents");
                builder.HasKey(static auditEvent => auditEvent.EventId);
                builder.Property(static auditEvent => auditEvent.SourceService).HasMaxLength(256).IsRequired();
                builder.Property(static auditEvent => auditEvent.EntityName).HasMaxLength(256).IsRequired();
                builder.Property(static auditEvent => auditEvent.EntityKey).HasMaxLength(256).IsRequired();
                builder.HasIndex(static auditEvent => new
                    {
                        auditEvent.SourceService,
                        auditEvent.EntityName,
                        auditEvent.EntityKey,
                        auditEvent.EntityVersion
                    }
                );
            }
        );

        modelBuilder.Entity<AuditPendingEvent>(
            builder =>
            {
                builder.ToTable("AuditPendingEvents");
                builder.HasKey(static message => message.Id);
                builder.Property(static message => message.SourceService).HasMaxLength(256).IsRequired();
                builder.Property(static message => message.EntityName).HasMaxLength(256).IsRequired();
                builder.Property(static message => message.EntityKey).HasMaxLength(256).IsRequired();
                builder.Property(static message => message.PayloadJson).IsRequired();
                builder.HasIndex(static message => message.EventId).IsUnique();
                builder.HasIndex(static message => new
                    {
                        message.SourceService,
                        message.EntityName,
                        message.EntityKey,
                        message.EntityVersion
                    }
                ).IsUnique();
            }
        );

        return modelBuilder;
    }
}
