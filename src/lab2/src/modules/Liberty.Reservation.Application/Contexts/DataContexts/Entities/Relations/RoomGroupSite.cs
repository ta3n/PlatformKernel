using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class RoomGroupSite : EntityRelation
{
    public long RoomGroupId { get; set; }
    public RoomGroup? RoomGroup { get; set; }

    public long SiteId { get; set; }
    public Site? Site { get; set; }

    /// <summary>
    /// 在庫設定期間の自動延長を利用するか
    /// </summary>
    public bool UseAutoExtend { get; set; }

    /// <summary>
    /// 在庫設定期間の毎月〇日の自動延長
    /// </summary>
    public int? AutoExtendEveryMonthDay { get; set; }

    /// <summary>
    /// 在庫設定期間を何カ月先まで延長するか？
    /// </summary>
    public int? AutoExtendMonth { get; set; }
}
