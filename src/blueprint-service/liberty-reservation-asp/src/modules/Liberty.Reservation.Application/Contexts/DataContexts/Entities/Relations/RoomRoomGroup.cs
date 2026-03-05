using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 部屋-部屋グループ
/// </summary>
public class RoomRoomGroup : EntityRelation
{
    /// <summary>
    /// 部屋ID
    /// </summary>
    public long RoomId { get; set; }

    /// <summary>
    /// 部屋
    /// </summary>
    public Room? Room { get; set; }

    /// <summary>
    /// 部屋グループID
    /// </summary>
    public long RoomGroupId { get; set; }

    /// <summary>
    /// 部屋グループ
    /// </summary>
    public RoomGroup? RoomGroup { get; set; }
}
