using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class SiteConfiguration : BaseDataEntityTypeConfiguration<Site>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Site> builder
    )
    {
        builder.ToTable("Site", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

        builder
            .HasMany(c => c.FacilitySites)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDatePriceData)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.RoomGroupSites)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDates)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.PlanRoomGroupSites)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDates)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteDiscountData)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.PlanRoomGroupSitePersonAgeTypes)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.PlanSites)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);

        builder
            .HasMany(c => c.SitePointRates)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId);
    }
}
