using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// メディア・部屋リレーション
/// </summary>
public class FileRoom : EntityRelation
{
    /// <summary>
    /// ファイルID
    /// </summary>
    public long FileId { get; set; }

    /// <summary>
    /// ファイル
    /// </summary>
    public File? File { get; set; }

    /// <summary>
    /// 部屋ID
    /// </summary>
    public long RoomId { get; set; }

    /// <summary>
    /// 部屋
    /// </summary>
    public Room? Room { get; set; }

    public int Index { get; set; }
}
