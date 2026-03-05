using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
///  入湯税グループ・対象料金設定リレーション
/// </summary>
public class SpaTaxGroupSpaTaxData : EntityRelation
{
    public long SpaTaxGroupId { get; set; }

    public SpaTaxGroup? SpaTaxGroup { get; set; }

    public long SpaTaxDataId { get; set; }

    public SpaTaxData? SpaTaxData { get; set; }
}
