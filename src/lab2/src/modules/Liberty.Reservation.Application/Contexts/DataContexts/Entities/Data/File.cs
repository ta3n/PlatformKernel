using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// ファイル情報
/// </summary>
public class File : EntityData
{
    public string? Secret { get; set; }
    public string? Extension { get; set; }

    /// <summary>
    /// コンテントタイプ
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// ファイルサイズ
    /// </summary>
    public float FileSize { get; set; }

    public string? Encrypt { get; set; }

    public string? Description { get; set; }
    public string? Tag { get; set; }

    /// <summary>
    /// 施設メディアリレーション
    /// </summary>
    public ICollection<FacilityFile>? FacilityFiles { get; set; }

    /// <summary>
    /// メディア部屋リレーション
    /// </summary>
    public ICollection<FileRoomGroup>? FileRoomGroups { get; set; }

    public ICollection<FileRoom>? FileRooms { get; set; }

    /// <summary>
    /// メディアファイルリレーション
    /// </summary>
    public ICollection<FilePlan>? FilePlans { get; set; }

    /// <summary>
    /// オプションアイテム-画像リレーション
    /// </summary>
    public ICollection<FileOptionItem>? FileOptionItems { get; set; }

    /// <summary>
    /// メディア区分リレーション
    /// </summary>
    public ICollection<FileCategory>? FileCategories { get; set; }
}
