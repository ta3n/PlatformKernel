using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 食事種別 ex:食事なし、朝食のみ、夕食のみ
/// </summary>
public class MealType : EntityData
{
    /// <summary>
    /// 食事種別名
    /// </summary>
    public string? Name { get; set; }

    public ICollection<PlanMealType>? PlanMealTypes { get; set; }
}
