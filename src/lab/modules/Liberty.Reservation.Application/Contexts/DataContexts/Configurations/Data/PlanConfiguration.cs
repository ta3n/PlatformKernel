using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PlanConfiguration : BaseDataEntityTypeConfiguration<Plan>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Plan> builder
    )
    {
        builder.ToTable("Plan", DbConfiguration.DefaultSchema);

        builder.Ignore(t => t.Meta);

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

        // builder
        //     .HasMany(c => c.PlanCalendars)
        //     .WithOne(c => c.Plan)
        //     .HasForeignKey(c => c.PlanID);

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
