using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 単体部屋
/// </summary>
public class Room : EntityData
{
    /// <summary>
    /// 部屋名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 説明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 部屋定員 最小
    /// </summary>
    public int? CapacityMin { get; set; }

    /// <summary>
    /// 部屋定員 最大
    /// </summary>
    public int? CapacityMax { get; set; }

    /// <summary>
    /// 部屋広さ
    /// </summary>
    public float? Size { get; set; }

    /// <summary>
    ///  部屋広さ単位
    /// </summary>
    public RoomGroupSizeUnitTypes RoomGroupSizeUnitType { get; set; }

    public string? Tag { get; set; }

    /// <summary>
    /// 情報JSON
    /// </summary>
    public string MetaJson { get; set; } = "{}";

    public RoomMeta? Meta
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<RoomMeta>(MetaJson);
            }
            catch
            {
                // 変換できない場合、新しいオブジェクトを返す
                return new RoomMeta();
            }
        }
        set => MetaJson = JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// ファイルリレーション
    /// </summary>
    public ICollection<FileRoom>? FileRooms { get; set; }

    public ICollection<RoomRoomGroup>? RoomRoomGroups { get; set; }
}
