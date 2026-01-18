using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class PlanMealType : EntityRelation
{
    /// <summary>
    /// プランID
    /// </summary>
    public long PlanId { get; set; }

    /// <summary>
    /// プラン
    /// </summary>
    public Plan? Plan { get; set; }

    /// <summary>
    /// 食事種別ID
    /// </summary>
    public long MealTypeId { get; set; }

    /// <summary>
    /// 食事
    /// </summary>
    public MealType? MealType { get; set; }

    public MealTypeEatTypes MealTypeEatType { get; set; }
}
