using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ReservationPlanRoomGroupAppDateConfiguration
    : BaseRelationEntityTypeConfiguration<ReservationPlanRoomGroupAppDate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ReservationPlanRoomGroupAppDate> builder
    )
    {
        builder.ToTable("reservation_plan_room_group_app_date", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.ReservationId,
                    c.PlanId,
                    c.RoomGroupId,
                    c.BookingDateId,
                    c.RoomGroupIndex
                }
            );

        builder
            .HasOne(sc => sc.Reservation)
            .WithMany(s => s.ReservationPlanRoomGroupAppDates)
            .HasForeignKey(sc => sc.ReservationId);

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.ReservationPlanRoomGroupAppDates)
            .HasForeignKey(sc => sc.PlanId);
        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.ReservationPlanRoomGroupAppDates)
            .HasForeignKey(sc => sc.RoomGroupId);
    }
}
