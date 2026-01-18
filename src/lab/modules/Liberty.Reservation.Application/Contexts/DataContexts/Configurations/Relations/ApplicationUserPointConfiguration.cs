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
        builder.ToTable("ApplicationUserPoint", DbConfiguration.DefaultSchema);

        builder.Property(x => x.UserId).HasColumnName("UserID");
        builder.Property(x => x.PointId).HasColumnName("PointID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.UserId,
                    c.PointId
                }
            );

        builder
            .HasOne(sc => sc.User)
            .WithMany(s => s.ApplicationUserPoints)
            .HasForeignKey(sc => sc.UserId);

        builder
            .HasOne(sc => sc.Point)
            .WithMany(s => s.ApplicationUserPoints)
            .HasForeignKey(sc => sc.PointId);
    }
}
