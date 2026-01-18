using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class OrderGmoPaymentResultRequest : EntityRelation
{
    public long OrderId { get; set; }
    public Order? Order { get; set; }

    public long GmoPaymentResultRequestId { get; set; }

    public GmoPaymentResultRequest? GmoPaymentResultRequest { get; set; }

    /// <summary>照合された日付</summary>
    public DateTime? ValidDate { get; set; }
}
