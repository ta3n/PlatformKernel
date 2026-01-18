using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class MealTypeConfiguration : BaseDataEntityTypeConfiguration<MealType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<MealType> builder
    )
    {
        builder.ToTable("meal_type", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.PlanMealTypes)
            .WithOne(c => c.MealType)
            .HasForeignKey(c => c.MealTypeId);
    }
}
