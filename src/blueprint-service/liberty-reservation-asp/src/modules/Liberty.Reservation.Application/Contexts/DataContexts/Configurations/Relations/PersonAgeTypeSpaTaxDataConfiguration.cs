using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PersonAgeTypeSpaTaxDataConfiguration : BaseRelationEntityTypeConfiguration<PersonAgeTypeSpaTaxData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PersonAgeTypeSpaTaxData> builder
    )
    {
        builder.ToTable("person_age_type_spa_tax_data", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PersonAgeTypeId,
                    c.SpaTaxDataId
                }
            );

        builder
            .HasOne(sc => sc.PersonAgeType)
            .WithMany(s => s.PersonAgeTypeSpaTaxDatas)
            .HasForeignKey(sc => sc.PersonAgeTypeId);

        builder
            .HasOne(sc => sc.SpaTaxData)
            .WithMany(s => s.PersonAgeTypeSpaTaxData)
            .HasForeignKey(sc => sc.SpaTaxDataId);
    }
}
