using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プラン質問リレーション
/// </summary>
public class PlanQuestion : EntityRelation
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
    /// 質問ID
    /// </summary>
    public long QuestionId { get; set; }

    /// <summary>
    /// 質問
    /// </summary>
    public Question? Question { get; set; }
}
