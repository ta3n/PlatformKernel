using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PlanConfiguration : BaseDataEntityTypeConfiguration<Plan>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Plan> builder
    )
    {
        builder.ToTable("plan", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Name)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Name)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

        builder.Property(e => e.Description)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Description)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Description)
            .HasMethod("GIN");

        builder.Property(e => e.Other)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Other)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Other)
            .HasMethod("GIN");

        builder.Property(e => e.Meal)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Meal)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Meal)
            .HasMethod("GIN");

        builder.Property(e => e.Payment)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Payment)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Payment)
            .HasMethod("GIN");

        builder.Property(e => e.Summary)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Summary)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Summary)
            .HasMethod("GIN");

        builder.Property(e => e.Tag)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Tag)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.Tag)
            .HasMethod("GIN");

        builder.Property(e => e.NameForImport)
            .HasColumnType(JsonbType)
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.NameForImport)
            .HasDefaultValueSql(JsonbDefaultValue);

        builder
            .HasIndex(x => x.NameForImport)
            .HasMethod("GIN");

        builder.Property(e => e.Meta)
            .HasColumnType(JsonbType)
            .HasDefaultValue(new PlanMeta())
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<PlanMeta>(v)
            );

        builder.HasIndex(e => e.Meta)
            .HasMethod("GIN");

        // 中間テーブルとの関係性登録

        builder
            .HasMany(c => c.FilePlans)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.FacilityPlans)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroups)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroupSites)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanCategories)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanMealTypes)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.FilePlans)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanOptionItems)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanQuestions)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDates)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDatePriceData)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteDiscountData)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroupSitePersonAgeTypes)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanRoomGroupCancellations)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.PlanSites)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);

        builder
            .HasMany(c => c.ReservationPlanRoomGroupAppDates)
            .WithOne(c => c.Plan)
            .HasForeignKey(c => c.PlanId);
    }
}
