using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanMealTypeConfiguration : BaseRelationEntityTypeConfiguration<PlanMealType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanMealType> builder
    )
    {
        builder.ToTable("plan_meal_type", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.MealTypeId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanMealTypes)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.MealType)
            .WithMany(s => s.PlanMealTypes)
            .HasForeignKey(sc => sc.MealTypeId);
    }
}
