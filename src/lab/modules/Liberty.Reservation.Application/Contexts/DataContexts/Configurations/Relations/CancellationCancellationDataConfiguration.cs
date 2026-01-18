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
        builder.ToTable("CancellationCancellationData", DbConfiguration.DefaultSchema);

        builder.Property(x => x.CancellationId).HasColumnName("CancellationID");
        builder.Property(x => x.CancellationDataId).HasColumnName("CancellationDataID");

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
