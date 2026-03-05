using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// ポイント
/// </summary>
public class Point : EntityData
{
    /// <summary>
    /// 種別
    /// </summary>
    public PointTypes PointType { get; set; }

    /// <summary>
    /// 獲得ポイント
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// ポイント残高
    ///
    /// 紐づくポイントを加味した残高を算出
    /// </summary>
    public int Remain
    {
        get
        {
            // ポイント種別がIncomeでなければ支出なのでRemainは0
            var remain = PointType == PointTypes.Income ? Value : 0;

            if (Children is { Count: > 0 })
            {
                remain -= Children
                    .Where(
                        a => a.PointType is PointTypes.Spend or PointTypes.Expire or PointTypes.Cancelled
                    )
                    .Sum(a => a.Value);
            }

            if (remain < 0)
            {
                remain = 0;
            }

            return remain;
        }
    }

    /// <summary>
    /// ポイント付与日
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 失効日
    /// </summary>
    public long ExpiredDate { get; set; }

    /// <summary>
    /// キャンセル処理日時
    /// </summary>
    public DateTime CancelledDate { get; set; }

    /// <summary>
    /// 運用可能開始日時
    /// ex:予約申し込み時にポイントは付与するが、実際に利用可能になるのはチェックアウトした段階などタイムラグがある
    /// </summary>
    public long EnabledStart { get; set; }

    /// <summary>
    /// 有効期間
    /// </summary>
    public int Expire { get; set; }

    [ForeignKey("Parent")]
    public long? ParentId { set; get; }

    /// <summary>
    /// 親Point
    /// </summary>
    public Point? Parent { get; set; }

    /// <summary>
    /// 関連するポイント
    /// 自信がIncomeの場合支払い分のSpendが加わる
    /// </summary>
    public ICollection<Point>? Children { get; set; }

    public ICollection<ApplicationUserPoint>? ApplicationUserPoints { get; set; }

    public ICollection<ReservationPoint>? ReservationPoints { get; set; }

    /// <summary>
    /// ポイント利用最終日
    /// </summary>
    public long ExpirationDate
    {
        get
        {
            // note: 有効期間はポイント発生日を含むため-1が必要
            //          例) 有効期間:1日の場合、ポイント発生日のみ有効 ⇒ AddDays(0)にする必要がある
            var expirationDate = EnabledStart + Expire - 1;
            return expirationDate;
        }
    }

    ///// <summary>
    ///// ポイント利用開始日
    ///// </summary>
    //public DateTime EnabledStartDate
    //{
    //    get
    //    {
    //        if (this.EnabledStart < 1) return DateTime.MinValue;
    //        return AppDate.GetDateTime(this.EnabledStart);
    //    }
    //}

    ///// <summary>
    ///// ポイント利用最終日
    ///// </summary>
    //public DateTime ExpirationDate
    //{
    //    get
    //    {
    //        // note: 有効期間はポイント発生日を含むため-1が必要
    //        //          例) 有効期間:1日の場合、ポイント発生日のみ有効 ⇒ AddDays(0)にする必要がある
    //        var expirationDate = this.EnabledStartDate.AddDays(this.Expire - 1);
    //        return expirationDate;
    //    }
    //}

    ///// <summary>
    ///// ポイントが使えるか判定
    /////
    ///// 判定対象日がポイント有効期間内か？
    ///// </summary>
    ///// <param name="targetDate"></param>
    ///// <returns></returns>
    //public bool CanUsePoint(DateTime targetDate)
    //{
    //    var result = Validate.IsInDate(targetDate, this.EnabledStartDate, this.ExpirationDate);
    //    return result;
    //}

    public bool IsExpire(
        long date
    )
    {
        return date > EnabledStart + Expire;
    }

    public void Exired(
        long date
    )
    {
        PointType = PointTypes.Expire;
        ExpiredDate = date;
    }

    public void Cancel(
        DateTime date
    )
    {
        CancelledDate = date;

        PointType = PointTypes.Cancelled;
    }
}
