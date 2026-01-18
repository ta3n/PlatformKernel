using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ApplicationUserFavorite : EntityRelation
{
    public long UserId { get; set; }
    public User? User { get; set; }

    public long FavoriteId { get; set; }
    public Favorite? Favorite { get; set; }

    public ApplicationUserFavorite()
    {
    }

    public ApplicationUserFavorite(
        User user,
        Favorite favorite
    )
    {
        UserId = user.Id;
        User = user;
        FavoriteId = favorite.Id;
        Favorite = favorite;
    }
}
