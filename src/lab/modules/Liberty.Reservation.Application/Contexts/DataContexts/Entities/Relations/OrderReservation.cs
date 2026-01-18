using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class OrderReservation : EntityRelation
{
    public long OrderId { get; set; }
    public Order? Order { get; set; }

    /// <summary>
    /// 性別名
    /// </summary>
    public long ReservationId { get; set; }

    public Data.Reservation? Reservation { get; set; }

    public OrderReservation()
    {
    }

    public OrderReservation(
        Order order,
        Data.Reservation reservation
    )
    {
        OrderId = order.Id;
        Order = order;
        ReservationId = reservation.Id;
        Reservation = reservation;
    }

    public bool IsExpired(
        DateTime now,
        int expireMinute
    )
    {
        // 一定時間経過しているか？
        return Order?.IsExpired(now, expireMinute) ?? false;
    }
}
