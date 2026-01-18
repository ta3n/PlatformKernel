using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class UserConfiguration : BaseDataEntityTypeConfiguration<User>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<User> builder
    )
    {
        builder.ToTable("User", DbConfiguration.DefaultSchema);

        // MySQLはコレを入れないとエラー: Specified key was too long; max key length is 3072 bytes
        builder.Property(u => u.Id).HasMaxLength(36);

        // ロジック上で必要なためテーブルには不要
        builder.Ignore(t => t.Roles);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.ApplicationUserPoints)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.ApplicationUserLoginHistories)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.ApplicationUserFavorites)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        builder
            .HasMany(c => c.FacilityApplicationUsers)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);
    }
}
