using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Test.Service.Domain;

namespace SharedKernel.BulkInsert.Test.Service.Data;

public sealed class BulkInsertLabDbContext(
    DbContextOptions<BulkInsertLabDbContext> options
) : DbContext(options)
{
    public DbSet<LabOrder> Orders => Set<LabOrder>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        modelBuilder.Entity<LabOrder>(
            builder =>
            {
                builder.ToTable(BulkInsertLabDatabase.Table, BulkInsertLabDatabase.Schema);

                builder.HasKey(order => order.Id);

                builder.Property(order => order.Id)
                    .HasColumnName("id")
                    .UseIdentityByDefaultColumn();
                builder.Property(order => order.ExternalId)
                    .HasColumnName("external_id")
                    .HasMaxLength(128);
                builder.Property(order => order.Quantity)
                    .HasColumnName("quantity");
                builder.Property(order => order.CreatedAtUtc)
                    .HasColumnName("created_at_utc");
                builder.Property(order => order.Source)
                    .HasColumnName("source")
                    .HasMaxLength(64);
            }
        );
    }
}
