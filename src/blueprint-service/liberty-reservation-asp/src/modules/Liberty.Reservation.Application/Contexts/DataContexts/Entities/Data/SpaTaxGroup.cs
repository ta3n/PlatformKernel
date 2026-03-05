using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 入湯税期間グループ
/// </summary>
public class SpaTaxGroup : EntityData
{
    /// <summary>
    /// 合計料金に入湯税を含めるか？（自動請求有効・無効に相当）
    /// </summary>
    public bool IsIncludeTotalFee { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// 施設・入湯税期間グループリレーション
    /// </summary>
    public ICollection<FacilitySpaTaxGroup>? FacilitySpaTaxGroups { get; set; }

    public ICollection<SpaTaxGroupSpaTaxData>? SpaTaxGroupSpaTaxData { get; set; }
}
