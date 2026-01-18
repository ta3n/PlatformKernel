using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityPlanConfiguration : BaseRelationEntityTypeConfiguration<FacilityPlan>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityPlan> builder
    )
    {
        builder.ToTable("FacilityPlan", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.PlanId).HasColumnName("PlanID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.PlanId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityPlans)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.FacilityPlans)
            .HasForeignKey(sc => sc.PlanId);
    }
}
