using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityAppDateTypeConfiguration : BaseRelationEntityTypeConfiguration<FacilityAppDateType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityAppDateType> builder
    )
    {
        builder.ToTable("FacilityAppDateType", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.AppDateTypeId).HasColumnName("AppDateTypeID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.AppDateTypeId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityAppDateTypes)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.AppDateType)
            .WithMany(s => s.FacilityAppDateTypes)
            .HasForeignKey(sc => sc.AppDateTypeId);
    }
}
