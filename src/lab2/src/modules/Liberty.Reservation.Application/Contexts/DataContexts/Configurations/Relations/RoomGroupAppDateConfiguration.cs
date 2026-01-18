using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupAppDateConfiguration : BaseRelationEntityTypeConfiguration<RoomGroupAppDate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupAppDate> builder
    )
    {
        builder.ToTable("room_group_app_date", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.RoomGroupId,
                    c.AppDateId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupAppDates)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.RoomGroupAppDates)
            .HasForeignKey(sc => sc.AppDateId);
    }
}
