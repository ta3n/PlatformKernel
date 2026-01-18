using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class AppDateTypeConfiguration : BaseDataEntityTypeConfiguration<AppDateType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<AppDateType> builder
    )
    {
        builder.ToTable("AppDateType", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.AppDateAppDateTypes)
            .WithOne(c => c.AppDateType)
            .HasForeignKey(c => c.AppDateTypeId);

        builder
            .HasMany(c => c.FacilityAppDateTypes)
            .WithOne(c => c.AppDateType)
            .HasForeignKey(c => c.AppDateTypeId);

        builder
            .HasMany(c => c.RoomGroupAppDateTypePriceData)
            .WithOne(c => c.AppDateType)
            .HasForeignKey(c => c.AppDateTypeId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.AppDateType)
            .HasForeignKey(c => c.AppDateTypeId);

        builder
            .HasMany(c => c.CalendarAppDateAppDateTypes)
            .WithOne(c => c.AppDateType)
            .HasForeignKey(c => c.AppDateTypeId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.AppDateType)
            .HasForeignKey(c => c.AppDateTypeId);
    }
}
