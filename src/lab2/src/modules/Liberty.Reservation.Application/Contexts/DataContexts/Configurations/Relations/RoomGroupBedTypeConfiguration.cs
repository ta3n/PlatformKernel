using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupBedTypeConfiguration : BaseRelationEntityTypeConfiguration<RoomGroupBedType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupBedType> builder
    )
    {
        builder.ToTable("room_group_bed_type", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.RoomGroupId,
                    c.BedTypeId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupBedTypes)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.BedType)
            .WithMany(s => s.RoomGroupBedType)
            .HasForeignKey(sc => sc.BedTypeId);
    }
}
