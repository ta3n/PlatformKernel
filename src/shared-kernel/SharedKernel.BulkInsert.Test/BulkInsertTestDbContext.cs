using Microsoft.EntityFrameworkCore;

namespace SharedKernel.BulkInsert.Test;

public sealed class BulkInsertTestDbContext(
    DbContextOptions<BulkInsertTestDbContext> options
) : DbContext(options)
{
    public DbSet<BulkInsertTestOrder> Orders => Set<BulkInsertTestOrder>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        modelBuilder.Entity<BulkInsertTestOrder>(
            builder =>
            {
                builder.ToTable("orders", "bulk_insert");

                builder.HasKey(order => order.Id);

                builder.Property(order => order.Id)
                    .HasColumnName("id")
                    .UseIdentityByDefaultColumn();
                builder.Property(order => order.ExternalId)
                    .HasColumnName("external_id")
                    .HasMaxLength(128);
                builder.Property(order => order.Quantity)
                    .HasColumnName("quantity");
                builder.Property(order => order.CreatedAt)
                    .HasColumnName("created_at");
            }
        );
    }
}

public sealed class BulkInsertTestOrder
{
    public long Id { get; set; }

    public required string ExternalId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }
}
