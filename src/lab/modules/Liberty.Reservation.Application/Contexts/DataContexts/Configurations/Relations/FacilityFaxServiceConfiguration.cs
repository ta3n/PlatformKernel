using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityFaxServiceConfiguration : BaseRelationEntityTypeConfiguration<FacilityFaxService>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityFaxService> builder
    )
    {
        builder.ToTable("FacilityFaxService", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.FaxServiceId).HasColumnName("FaxServiceID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.FaxServiceId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityFaxServices)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.FaxService)
            .WithMany(s => s.FacilityFaxServices)
            .HasForeignKey(sc => sc.FaxServiceId);
    }
}
