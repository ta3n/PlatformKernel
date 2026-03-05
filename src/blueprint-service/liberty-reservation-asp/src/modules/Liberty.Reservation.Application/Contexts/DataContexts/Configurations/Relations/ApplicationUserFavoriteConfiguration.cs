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
        builder.ToTable("application_user_favorite", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.UserCode,
                    c.FavoriteId
                }
            );

        builder
            .HasOne(sc => sc.Favorite)
            .WithMany(s => s.ApplicationUserFavorites)
            .HasForeignKey(sc => sc.FavoriteId);
    }
}
