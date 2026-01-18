using Liberty.Entity;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Models.Responses;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class GmoChangeTranReport : EntityData
{
    public string? OrderId { get; set; }
    public long ReservationId { get; set; }
    public string? AccessId { get; set; }
    public ChangeOrderRequest? Request { get; set; }
    public OnlinePaymentChangeResponse? Response { get; set; }
}
