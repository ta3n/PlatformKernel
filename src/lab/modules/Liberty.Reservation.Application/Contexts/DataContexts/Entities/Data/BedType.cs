using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

// 布団
//シングル
//セミダブル
//ダブル
//クイーン
//キング
//エクストラ
//その他
public class BedType : EntityData
{
    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///  単位:ex 組み・台
    /// </summary>
    public BedTypeUnitTypes BedTypeUnitType { get; set; }

    /// <summary>
    /// 部屋-ベッドタイプリリレーション
    /// </summary>
    public ICollection<RoomGroupBedType>? RoomGroupBedType { get; set; }
}
