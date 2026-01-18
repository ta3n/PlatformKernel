using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// キャンセル料
/// </summary>
public class Cancellation : EntityData
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    /// <summary>オンライン決済でキャンセル料の支払いが可能か？</summary>
    public bool CanOnLinePayment { get; set; }

    /// <summary>キャンセル確定日から〇日以内での支払い期限</summary>
    public int? PaymentLimit { get; set; }

    /// <summary>
    /// 情報JSON
    /// </summary>
    public string MetaJson { get; set; } = "{}";

    public CancellationMeta? Meta
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<CancellationMeta>(MetaJson);
            }
            catch
            {
                // 変換できない場合、新しいオブジェクトを返す
                return new CancellationMeta();
            }
        }
        set => MetaJson = JsonConvert.SerializeObject(value);
    }

    /// <summary>キャンセル規定取得 </summary>
    /// <param name="checkInDate"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    public CancellationCancellationData? GetCancellationCancellationData(
        DateTime checkInDate,
        DateTime date
    )
    {
        var span = checkInDate - date;
        var days = span.Days;
        // データの範囲外に収めさせるためマイナスの場合0でとみなす
        if (days < 0)
        {
            days = 0;
        }

        var cancellationData = CancellationCancellationDatas?
            .FirstOrDefault(
                x => x.CancellationData is not null && x.CancellationData.IsRange(days)
            );

        return cancellationData;
    }

    /// <summary>
    /// キャンセル料取得
    /// 該当しなければ0
    /// </summary>
    /// <param name="checkInDate"></param>
    /// <param name="date"></param>
    /// <param name="price"></param>
    /// <returns></returns>
    public int? GetCancellationPrice(
        int price,
        DateTime checkInDate,
        DateTime date
    )
    {
        var cancellationCancellationData = GetCancellationCancellationData(
            checkInDate,
            date
        );

        return cancellationCancellationData?.CancellationData?.Calc(price) ?? 0;
    }

    /// <summary>
    /// キャンセルーキャンセル規定詳細リレーション
    /// </summary>
    public ICollection<CancellationCancellationData>? CancellationCancellationDatas { get; set; }

    /// <summary>
    /// 施設ーキャンセル規定詳細リレーション
    /// </summary>
    public ICollection<FacilityCancellation>? FacilityCancellations { get; set; }

    /// <summary>
    /// プランー部屋ーキャンセル規定料リレーション
    /// </summary>
    public ICollection<PlanRoomGroupCancellation>? PlanRoomGroupCancellations { get; set; }
}
