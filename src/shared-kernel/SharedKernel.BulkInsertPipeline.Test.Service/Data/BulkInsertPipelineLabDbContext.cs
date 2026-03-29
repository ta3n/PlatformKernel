using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsertPipeline.Test.Service.Domain;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Data;

public sealed class BulkInsertPipelineLabDbContext(
    DbContextOptions<BulkInsertPipelineLabDbContext> options
) : DbContext(options)
{
    public DbSet<MetricReading> Metrics => Set<MetricReading>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        modelBuilder.Entity<MetricReading>(
            builder =>
            {
                builder.ToTable(BulkInsertPipelineLabDatabase.Table, BulkInsertPipelineLabDatabase.Schema);

                builder.HasKey(reading => reading.Id);

                builder.Property(reading => reading.Id)
                    .HasColumnName("id")
                    .UseIdentityByDefaultColumn();
                builder.Property(reading => reading.DeviceId)
                    .HasColumnName("device_id")
                    .HasMaxLength(128);
                builder.Property(reading => reading.OccurredAt)
                    .HasColumnName("occurred_at");
                builder.Property(reading => reading.Value)
                    .HasColumnName("value");
                builder.Property(reading => reading.Quality)
                    .HasColumnName("quality");
                builder.Property(reading => reading.Source)
                    .HasColumnName("source")
                    .HasMaxLength(64);
            }
        );
    }
}
