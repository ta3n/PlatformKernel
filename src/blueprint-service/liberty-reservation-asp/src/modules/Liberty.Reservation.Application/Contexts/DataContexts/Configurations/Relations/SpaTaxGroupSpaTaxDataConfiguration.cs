using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class SpaTaxGroupSpaTaxDataConfiguration : BaseRelationEntityTypeConfiguration<SpaTaxGroupSpaTaxData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<SpaTaxGroupSpaTaxData> builder
    )
    {
        builder.ToTable("spa_tax_group_spa_tax_data", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.SpaTaxGroupId,
                    c.SpaTaxDataId
                }
            );

        builder
            .HasOne(sc => sc.SpaTaxGroup)
            .WithMany(s => s.SpaTaxGroupSpaTaxData)
            .HasForeignKey(sc => sc.SpaTaxGroupId);

        builder
            .HasOne(sc => sc.SpaTaxData)
            .WithMany(s => s.SpaTaxGroupSpaTaxData)
            .HasForeignKey(sc => sc.SpaTaxDataId);
    }
}
