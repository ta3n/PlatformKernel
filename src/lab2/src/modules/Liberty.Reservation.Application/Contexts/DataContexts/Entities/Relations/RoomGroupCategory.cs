using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 部屋カテゴリ
/// </summary>
public class RoomGroupCategory : EntityRelation
{
    /// <summary>
    /// 部屋ID
    /// </summary>
    public long RoomGroupId { get; set; }

    /// <summary>
    /// 部屋
    /// </summary>
    public RoomGroup? RoomGroup { get; set; }

    /// <summary>
    /// カテゴリID
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// カテゴリ
    /// </summary>
    public Category? Category { get; set; }
}
