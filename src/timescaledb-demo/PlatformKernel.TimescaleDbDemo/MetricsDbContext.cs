using Microsoft.EntityFrameworkCore;

namespace PlatformKernel.TimescaleDbDemo;

public sealed class MetricsDbContext(
    DbContextOptions<MetricsDbContext> options
) : DbContext(options)
{
    public DbSet<MetricPoint> Metrics => Set<MetricPoint>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        var entity = modelBuilder.Entity<MetricPoint>();

        entity.ToTable("metrics");
        entity.HasKey(
            metric => new
            {
                metric.Time,
                metric.DeviceId
            }
        );

        entity.Property(metric => metric.Time)
            .HasColumnName("time");

        entity.Property(metric => metric.DeviceId)
            .HasColumnName("device_id");

        entity.Property(metric => metric.Cpu)
            .HasColumnName("cpu");

        entity.Property(metric => metric.Memory)
            .HasColumnName("memory");

        entity.Property(metric => metric.Temperature)
            .HasColumnName("temperature");

        entity.Property(metric => metric.Tags)
            .HasColumnName("tags")
            .HasColumnType("jsonb");
    }
}
