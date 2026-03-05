using Liberty.Entity;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// キャンセル料情報
/// </summary>
public class CancellationData : EntityData
{
    /// <summary>
    /// 宿泊日から〇日前から
    /// </summary>
    public int DayStart { get; set; }

    /// <summary>
    /// 宿泊日の〇日前まで
    /// </summary>
    public int DayEnd { get; set; }

    /// <summary>〇％</summary>
    public float Rate { get; set; }

    /// <summary>補足説明</summary>
    public MultilingualText? Description { get; set; }

    /// <summary>
    /// キャンセルーキャンセル詳細リレーション
    /// </summary>
    public ICollection<CancellationCancellationData>? CancellationCancellationData { get; set; }

    public bool IsRange(
        int day
    )
    {
        return DayStart <= day && DayEnd >= day;
    }

    public decimal Calc(
        decimal totalPrice
    )
    {
        // 端数切捨て(負はあり得ないが、当社「端数切捨て」なのでTruncateを採用
        return Math.Truncate(totalPrice / 100 * (decimal)Rate);
    }
}
