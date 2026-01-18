using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 注文
/// </summary>
public class Order : EntityData
{
    public DateTime OrderDateTime { get; set; }

    /// <summary>
    /// API連携用払い出しコード
    /// ex:GMOペイメントの場合「OrderID」(27桁)と紐づけ
    /// </summary>
    public string? ApiIssueCode { get; set; }

    /// <summary>
    /// 注文情報とのリレーション
    /// </summary>
    public ICollection<OrderReservation>? OrderReservations { get; set; }

    public ICollection<OrderGmoPaymentResultRequest>? OrderGMOPaymentResultRequests { get; set; }

    public Order()
    {
    }

    public Order(
        DateTime orderDate
    )
    {
        OrderDateTime = orderDate;
    }

    public bool IsExpired(
        DateTime now,
        int expireMinute
    )
    {
        return now > OrderDateTime.AddMinutes(expireMinute);
    }
}
