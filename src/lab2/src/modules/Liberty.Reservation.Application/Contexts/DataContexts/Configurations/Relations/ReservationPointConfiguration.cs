using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ReservationPointConfiguration : BaseRelationEntityTypeConfiguration<ReservationPoint>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ReservationPoint> builder
    )
    {
        builder.ToTable("reservation_point", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.ReservationId,
                    c.PointId
                }
            );

        builder
            .HasOne(sc => sc.Reservation)
            .WithMany(s => s.ReservationPoints)
            .HasForeignKey(sc => sc.ReservationId);

        builder
            .HasOne(sc => sc.Point)
            .WithMany(s => s.ReservationPoints)
            .HasForeignKey(sc => sc.PointId);
    }
}
