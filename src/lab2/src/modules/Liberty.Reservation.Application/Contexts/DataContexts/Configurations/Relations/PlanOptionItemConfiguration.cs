using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanOptionItemConfiguration : BaseRelationEntityTypeConfiguration<PlanOptionItem>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanOptionItem> builder
    )
    {
        builder.ToTable("plan_option_item", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.OptionItemId,
                    c.PlanOptionItemType
                }
            );

        builder
            .HasOne(sc => sc.OptionItem)
            .WithMany(s => s.PlanOptionItems)
            .HasForeignKey(sc => sc.OptionItemId);

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanOptionItems)
            .HasForeignKey(sc => sc.PlanId);
    }
}
