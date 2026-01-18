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
        builder.ToTable("ReservationPlanRoomGroupAppDate", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            //.HasKey(c => new { c.ReservationID, c.PlanID, c.RoomGroupID, c.AppDateID, c.RestIndex, c.RoomGroupIndex });
            .HasKey(
                c => new
                {
                    ReservationID = c.ReservationId,
                    PlanID = c.PlanId,
                    RoomGroupID = c.RoomGroupId,
                    AppDateID = c.AppDateId,
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

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.ReservationPlanRoomGroupAppDates)
            .HasForeignKey(sc => sc.AppDateId);
    }
}
