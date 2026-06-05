using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyAuditLogging(
        this ModelBuilder modelBuilder
    )
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<AuditLog>(
            builder =>
            {
                builder.ToTable("AuditLogs");
                builder.HasKey(static auditLog => auditLog.Id);
                builder.Property(static auditLog => auditLog.EntityName).HasMaxLength(256).IsRequired();
                builder.Property(static auditLog => auditLog.EntityId).HasMaxLength(256).IsRequired();
                builder.Property(static auditLog => auditLog.UserId).HasMaxLength(256).IsRequired();
                builder.Property(static auditLog => auditLog.UserName).HasMaxLength(256).IsRequired();
                builder.Property(static auditLog => auditLog.TenantId).HasMaxLength(256);
                builder.Property(static auditLog => auditLog.IpAddress).HasMaxLength(128);
                builder.Property(static auditLog => auditLog.TraceId).HasMaxLength(128);
                builder.Property(static auditLog => auditLog.Source).HasMaxLength(256).IsRequired();
                builder.Property(static auditLog => auditLog.ChangesJson).IsRequired();
                builder.Property(static auditLog => auditLog.MetadataJson).IsRequired();
                builder.Property(static auditLog => auditLog.PreviousHash).HasMaxLength(128);
                builder.Property(static auditLog => auditLog.Hash).HasMaxLength(128);
                builder.HasIndex(static auditLog => auditLog.TimestampUtc);
                builder.HasIndex(static auditLog => new { auditLog.EntityName, auditLog.EntityId });
            }
        );

        modelBuilder.Entity<AuditOutbox>(
            builder =>
            {
                builder.ToTable("AuditOutbox");
                builder.HasKey(static auditOutbox => auditOutbox.Id);
                builder.Property(static auditOutbox => auditOutbox.PayloadJson).IsRequired();
                builder.Property(static auditOutbox => auditOutbox.LastError).HasMaxLength(2048);
                builder.HasIndex(static auditOutbox => auditOutbox.ProcessedUtc);
            }
        );

        return modelBuilder;
    }
}
