using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プランに含ませる固定オプション
/// </summary>
public class PlanOptionItem : EntityRelation
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
    /// オプションアイテムID
    /// </summary>
    public long OptionItemId { get; set; }

    /// <summary>
    /// オプションアイテム
    /// </summary>
    public OptionItem? OptionItem { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int? Number { get; set; }

    public OptionItemTargets OptionItemTarget { get; set; }

    /// <summary>固定オプション、任意オプション</summary>
    public PlanOptionItemTypes PlanOptionItemType { get; set; }
}
