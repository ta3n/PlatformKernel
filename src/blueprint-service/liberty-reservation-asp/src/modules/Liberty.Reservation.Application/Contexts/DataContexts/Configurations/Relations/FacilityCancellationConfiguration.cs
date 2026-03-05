using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityCancellationConfiguration : BaseRelationEntityTypeConfiguration<FacilityCancellation>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityCancellation> builder
    )
    {
        builder.ToTable("facility_cancellation", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.CancellationId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityCancellations)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.Cancellation)
            .WithMany(s => s.FacilityCancellations)
            .HasForeignKey(sc => sc.CancellationId);
    }
}
