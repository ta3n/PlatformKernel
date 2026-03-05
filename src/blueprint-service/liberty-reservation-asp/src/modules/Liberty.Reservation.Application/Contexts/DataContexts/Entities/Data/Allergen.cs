using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// アレルゲン
/// </summary>
public class Allergen : EntityData
{
    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// アレルゲン対応
    /// </summary>
    public ICollection<FacilityAllergen>? FacilityAllergens { get; set; }
}
