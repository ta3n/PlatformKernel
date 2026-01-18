using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PointConfiguration : BaseDataEntityTypeConfiguration<Point>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Point> builder
    )
    {
        builder.ToTable("Point", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.ApplicationUserPoints)
            .WithOne(c => c.Point)
            .HasForeignKey(c => c.PointId);

        builder
            .HasMany(c => c.ReservationPoints)
            .WithOne(c => c.Point)
            .HasForeignKey(c => c.PointId);

        builder
            .HasMany(c => c.ReservationPoints)
            .WithOne(c => c.Point)
            .HasForeignKey(c => c.PointId);
    }
}
