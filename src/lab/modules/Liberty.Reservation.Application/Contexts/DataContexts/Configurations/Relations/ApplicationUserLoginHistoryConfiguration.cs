using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ApplicationUserLoginHistoryConfiguration : BaseRelationEntityTypeConfiguration<ApplicationUserLoginHistory>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ApplicationUserLoginHistory> builder
    )
    {
        builder.ToTable("ApplicationUserLoginHistory", DbConfiguration.DefaultSchema);

        builder.Property(x => x.UserId).HasColumnName("UserID");
        builder.Property(x => x.LoginHistoryId).HasColumnName("LoginHistoryID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.UserId,
                    c.LoginHistoryId
                }
            );

        builder
            .HasOne(sc => sc.User)
            .WithMany(s => s.ApplicationUserLoginHistories)
            .HasForeignKey(sc => sc.UserId);

        builder
            .HasOne(sc => sc.LoginHistory)
            .WithMany(s => s.ApplicationUserLoginHistories)
            .HasForeignKey(sc => sc.LoginHistoryId);
    }
}
