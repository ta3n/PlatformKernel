using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class RoomGroupConfiguration : BaseDataEntityTypeConfiguration<RoomGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroup> builder
    )
    {
        builder.ToTable("room_group", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

        builder.Property(e => e.Name)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Name)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

        builder.Property(e => e.Overview)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Overview)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Overview)
            .HasMethod("GIN");

        builder.Property(e => e.Description)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Description)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Description)
            .HasMethod("GIN");

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
            .HasMany(c => c.ReservationRoomGroupAppDatePersonAgeTypes)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .HasMany(c => c.ReservationPlanRoomGroupAppDates)
            .WithOne(c => c.RoomGroup)
            .HasForeignKey(c => c.RoomGroupId);

        builder
            .Property(r => r.IsDescriptionVisible)
            .HasDefaultValue(true);

        builder
            .Property(r => r.IsOverviewVisible)
            .HasDefaultValue(true);

        builder
            .Property(r => r.IsRoomSizeVisible)
            .HasDefaultValue(true);

        builder
            .Property(r => r.IsBedTypeVisible)
            .HasDefaultValue(true);
    }
}
