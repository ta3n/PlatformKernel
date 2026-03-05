using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupCategoryConfiguration : BaseRelationEntityTypeConfiguration<RoomGroupCategory>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupCategory> builder
    )
    {
        builder.ToTable("room_group_category", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.RoomGroupId,
                    c.CategoryId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupCategories)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Category)
            .WithMany(s => s.RoomGroupCategories)
            .HasForeignKey(sc => sc.CategoryId);
    }
}
