using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class ApplicationUserFavoriteConfiguration : BaseRelationEntityTypeConfiguration<ApplicationUserFavorite>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<ApplicationUserFavorite> builder
    )
    {
        builder.ToTable("ApplicationUserFavorite", DbConfiguration.DefaultSchema);

        builder.Property(x => x.UserId).HasColumnName("UserID");
        builder.Property(x => x.FavoriteId).HasColumnName("FavoriteID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.UserId,
                    c.FavoriteId
                }
            );

        builder
            .HasOne(sc => sc.User)
            .WithMany(s => s.ApplicationUserFavorites)
            .HasForeignKey(sc => sc.UserId);

        builder
            .HasOne(sc => sc.Favorite)
            .WithMany(s => s.ApplicationUserFavorites)
            .HasForeignKey(sc => sc.FavoriteId);
    }
}
