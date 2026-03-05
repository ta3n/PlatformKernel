using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ApplicationUserPointConfiguration : BaseRelationEntityTypeConfiguration<ApplicationUserPoint>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ApplicationUserPoint> builder
    )
    {
        builder.ToTable("application_user_point", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.UserCode,
                    c.PointId
                }
            );

        builder
            .HasOne(sc => sc.Point)
            .WithMany(s => s.ApplicationUserPoints)
            .HasForeignKey(sc => sc.PointId);
    }
}
