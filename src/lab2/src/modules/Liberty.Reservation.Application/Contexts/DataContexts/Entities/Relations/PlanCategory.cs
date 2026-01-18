using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プランカテゴリ
/// </summary>
public class PlanCategory : EntityRelation
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
    ///  カテゴリID
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// カテゴリ
    /// </summary>
    public Category? Category { get; set; }
}
