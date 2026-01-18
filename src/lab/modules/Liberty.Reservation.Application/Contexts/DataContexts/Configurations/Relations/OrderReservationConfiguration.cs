using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class OrderReservationConfiguration : BaseRelationEntityTypeConfiguration<OrderReservation>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<OrderReservation> builder
    )
    {
        builder.ToTable("OrderReservation", DbConfiguration.DefaultSchema);

        builder.Property(x => x.OrderId).HasColumnName("OrderID");
        builder.Property(x => x.ReservationId).HasColumnName("ReservationID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.OrderId,
                    c.ReservationId
                }
            );

        builder
            .HasOne(sc => sc.Order)
            .WithMany(s => s.OrderReservations)
            .HasForeignKey(sc => sc.OrderId);

        builder
            .HasOne(sc => sc.Reservation)
            .WithMany(s => s.OrderReservations)
            .HasForeignKey(sc => sc.ReservationId);
    }
}
