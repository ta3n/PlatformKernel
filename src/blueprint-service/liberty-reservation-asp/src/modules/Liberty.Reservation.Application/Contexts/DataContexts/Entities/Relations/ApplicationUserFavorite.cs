using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ApplicationUserFavorite : EntityRelation
{
    public required string UserCode { get; set; }

    public long FavoriteId { get; set; }
    public Favorite? Favorite { get; set; }
}
