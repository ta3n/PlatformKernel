using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class AppDateAppDateDataConfiguration : BaseRelationEntityTypeConfiguration<AppDateAppDateData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<AppDateAppDateData> builder
    )
    {
        builder.ToTable("AppDateAppDateData", DbConfiguration.DefaultSchema);

        builder.Property(x => x.AppDateId).HasColumnName("AppDateID");
        builder.Property(x => x.AppDateDataId).HasColumnName("AppDateDataID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.AppDateId,
                    c.AppDateDataId
                }
            );

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.AppDateAppDateDatas)
            .HasForeignKey(sc => sc.AppDateId);

        builder
            .HasOne(sc => sc.AppDateData)
            .WithMany(s => s.AppDateAppDateData)
            .HasForeignKey(sc => sc.AppDateDataId);
    }
}
