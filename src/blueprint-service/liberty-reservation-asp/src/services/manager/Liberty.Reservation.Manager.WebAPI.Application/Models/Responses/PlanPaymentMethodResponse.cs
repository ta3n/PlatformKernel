namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanPaymentMethodResponse
{
    public long Id { get; init; }
    public bool IsOnSidePayment { get; init; }
    public bool IsOnLinePayment { get; init; }
}
