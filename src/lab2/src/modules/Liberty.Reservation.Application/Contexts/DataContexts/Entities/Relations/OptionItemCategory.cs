using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 別注料理等オプション商品カテゴリ
/// </summary>
public class OptionItemCategory : EntityRelation
{
    /// <summary>
    /// オプションアイテムID
    /// </summary>
    public long OptionItemId { get; set; }

    /// <summary>
    /// オプションアイテム
    /// </summary>
    public OptionItem? OptionItem { get; set; }

    /// <summary>
    /// カテゴリID
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// カテゴリ
    /// </summary>
    public Category? Category { get; set; }
}
