using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class SiteConfiguration : BaseDataEntityTypeConfiguration<Site>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Site> builder
    )
    {
        builder.ToTable("site", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Meta)
            .HasColumnType("jsonb")
            .HasDefaultValue(new SiteMeta())
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<SiteMeta>(v)
            );

        builder.Property(e => e.Name)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Name)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

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
