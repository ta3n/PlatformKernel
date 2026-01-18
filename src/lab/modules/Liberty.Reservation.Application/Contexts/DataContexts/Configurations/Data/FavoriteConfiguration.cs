using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class FavoriteConfiguration : BaseDataEntityTypeConfiguration<Favorite>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Favorite> builder
    )
    {
        builder.ToTable("Favorite", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.ApplicationUserFavorites)
            .WithOne(c => c.Favorite)
            .HasForeignKey(c => c.FavoriteId);
    }
}
