using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityPersonAgeTypeConfiguration : BaseRelationEntityTypeConfiguration<FacilityPersonAgeType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityPersonAgeType> builder
    )
    {
        builder.ToTable("FacilityPersonAgeType", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.PersonAgeTypeId).HasColumnName("PersonAgeTypeID");

        //中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.PersonAgeTypeId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityPersonAgeTypes)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.PersonAgeType)
            .WithMany(s => s.FacilityPersonAgeTypes)
            .HasForeignKey(sc => sc.PersonAgeTypeId);
    }
}
