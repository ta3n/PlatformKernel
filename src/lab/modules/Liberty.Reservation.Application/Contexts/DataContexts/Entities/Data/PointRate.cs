using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// ポイント付与率設定
/// </summary>
public class PointRate : EntityData
{
    public string? Name { get; set; }

    public float Rate { get; set; }
    public int Expire { get; set; }

    /// <summary>
    /// 運用開始日時
    /// </summary>
    public long EnabledStart { get; set; }

    /// <summary>
    /// 運用終了日時
    /// </summary>
    public long EnabledEnd { get; set; }

    /// <summary>
    /// ポイント取引種別
    /// どの取引に対しての設定かを指定します
    /// </summary>
    public PointRateTypes PointRateType { get; set; }

    public ICollection<SitePointRate>? SitePointRates { get; set; }
}
