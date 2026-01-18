using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class RoomGroupConfiguration : BaseDataEntityTypeConfiguration<RoomGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroup> builder
    )
    {
        builder.ToTable("RoomGroup", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

        builder
            .HasMany(c => c.FileRoomGroups)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomRoomGroups)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.FacilityRoomGroups)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupAppDateTypePriceData)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupBedTypes)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupCategories)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupAppDates)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDatePriceData)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupSites)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDates)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroups)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroupSites)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDates)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDatePriceData)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteDiscountData)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroupSitePersonAgeTypes)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.PlanRoomGroupCancellations)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDateOptionItems)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.ReservationRoomGroupAppDatePersonAgeTypes)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.ReservationPlanRoomGroupAppDates)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);
    }
}
