using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 部屋ベッド種別リレーション
/// </summary>
public class RoomGroupBedType : EntityRelation
{
    public long RoomGroupId { get; set; }

    public RoomGroup? RoomGroup { get; set; }

    public long BedTypeId { get; set; }

    public BedType? BedType { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int? Number { get; set; }
}
