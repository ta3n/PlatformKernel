using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ReservationRoomGroupAppDateOptionItemConfiguration
    : BaseRelationEntityTypeConfiguration<ReservationRoomGroupAppDateOptionItem>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ReservationRoomGroupAppDateOptionItem> builder
    )
    {
        builder.ToTable("ReservationRoomGroupAppDateOptionItem", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    ReservationID = c.ReservationId,
                    RoomGroupID = c.RoomGroupId,
                    AppDateID = c.AppDateId,
                    OptionItemID = c.OptionItemId,
                    c.RoomGroupIndex
                }
            );

        builder
            .HasOne(sc => sc.Reservation)
            .WithMany(s => s.ReservationRoomGroupAppDateOptionItems)
            .HasForeignKey(sc => sc.ReservationId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.ReservationRoomGroupAppDateOptionItems)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.ReservationRoomGroupAppDateOptionItems)
            .HasForeignKey(sc => sc.AppDateId);

        builder
            .HasOne(sc => sc.OptionItem)
            .WithMany(s => s.ReservationRoomGroupAppDateOptionItems)
            .HasForeignKey(sc => sc.OptionItemId);
    }
}
