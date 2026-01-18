using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// メディア・プランリレーション
/// </summary>
public class FilePlan : EntityRelation
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
    public long PlanId { get; set; }

    /// <summary>
    /// プラン
    /// </summary>
    public Plan? Plan { get; set; }

    /// <summary>
    /// ファイル設定順番
    /// </summary>
    public int Index { get; set; }
}
