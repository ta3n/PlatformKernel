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
        builder.ToTable("PlanMealType", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.MealTypeId).HasColumnName("MealTypeID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    PlanID = c.PlanId,
                    MealTypeID = c.MealTypeId
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
