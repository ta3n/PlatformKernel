using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// オプションアイテム-在庫情報
/// </summary>
public class OptionItemAppDate : EntityRelation
{
    /// <summary>
    /// オプションアイテムID
    /// </summary>
    public long OptionItemId { get; set; }

    /// <summary>
    /// オプションアイテム
    /// </summary>
    public OptionItem? OptionItem { get; set; }

    public long AppDateId { get; set; }

    public AppDate? AppDate { get; set; }

    /// <summary>
    /// 売り止めとするか？
    /// </summary>
    public bool IsNotSelled { get; set; }

    /// <summary>
    /// 販売数
    /// </summary>
    public int? SellNumber { get; set; }

    /// <summary>
    /// 在庫数
    /// </summary>
    public int? RemainNumber
    {
        get
        {
            if (SellNumber == null)
            {
                return null;
            }

            var number = SellNumber.Value;
            var remainNumber = number - ReservedNumber;

            return remainNumber;
        }
    }

    /// <summary>
    /// 予約数
    /// 予約成立した、オンライン支払未決済状態を予約数としてみなす
    /// </summary>
    public int ReservedNumber
    {
        get
        {
            var number = ReservationRoomGroupAppDateOptionItems?
                    .Where(
                        a => a.Reservation?.IsReserved ?? false
                    )
                    .Sum(a => a.Number)
                ?? 0;

            return number;
        }
    }

    /// <summary>
    /// 現在の予約
    /// </summary>
    public ICollection<ReservationRoomGroupAppDateOptionItem>? ReservationRoomGroupAppDateOptionItems { get; set; }
}
