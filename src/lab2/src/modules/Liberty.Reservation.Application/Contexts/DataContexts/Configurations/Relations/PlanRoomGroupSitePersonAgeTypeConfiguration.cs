using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupSitePersonAgeTypeConfiguration
    : BaseRelationEntityTypeConfiguration<PlanRoomGroupSitePersonAgeType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupSitePersonAgeType> builder
    )
    {
        builder.ToTable("plan_room_group_site_person_age_type", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId,
                    c.PersonAgeTypeId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupSitePersonAgeTypes)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupSitePersonAgeTypes)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.PlanRoomGroupSitePersonAgeTypes)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.PersonAgeType)
            .WithMany(s => s.PlanRoomGroupSitePersonAgeTypes)
            .HasForeignKey(sc => sc.PersonAgeTypeId);
    }
}
