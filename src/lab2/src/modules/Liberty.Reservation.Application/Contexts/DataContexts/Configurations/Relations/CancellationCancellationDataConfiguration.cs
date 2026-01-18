using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class CancellationCancellationDataConfiguration
    : BaseRelationEntityTypeConfiguration<CancellationCancellationData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<CancellationCancellationData> builder
    )
    {
        builder.ToTable("cancellation_cancellation_data", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.CancellationId,
                    c.CancellationDataId
                }
            );

        builder
            .HasOne(sc => sc.Cancellation)
            .WithMany(s => s.CancellationCancellationDatas)
            .HasForeignKey(sc => sc.CancellationId);

        builder
            .HasOne(sc => sc.CancellationData)
            .WithMany(s => s.CancellationCancellationData)
            .HasForeignKey(sc => sc.CancellationDataId);
    }
}
