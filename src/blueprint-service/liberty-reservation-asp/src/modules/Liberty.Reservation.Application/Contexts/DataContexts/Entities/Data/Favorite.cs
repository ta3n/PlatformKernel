using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// お気に入り
/// </summary>
public class Favorite : EntityData
{
    public long? PlanId { get; set; }
    public Plan? Plan { get; set; }

    public long? FacilityId { get; set; }
    public Facility? Facility { get; set; }

    /// <summary>
    /// アプリケーションユーザーお気に入りリレーション
    /// </summary>
    public ICollection<ApplicationUserFavorite>? ApplicationUserFavorites { get; set; }
}
