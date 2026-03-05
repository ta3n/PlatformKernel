using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 別注料理等オプション商品カテゴリ
/// </summary>
public class OptionItemQuestion : EntityRelation
{
    /// <summary>
    /// オプションアイテムID
    /// </summary>
    public long OptionItemId { get; set; }

    /// <summary>
    /// オプションアイテム
    /// </summary>
    public OptionItem? OptionItem { get; set; }

    public long QuestionId { get; set; }

    /// <summary>
    /// 質問
    /// </summary>
    public Question? Question { get; set; }
}
