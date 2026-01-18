namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityDetailPaymentMethodResponse
{
    public string? Code { get; init; }
    public string? OnSidePaymentComment { get; init; }
    public string? OnLinePaymentComment { get; init; }
    public string? PaymentComment { get; init; }
    public bool IsOnSidePayment { get; init; }
    public bool IsOnLinePayment { get; init; }
    public bool CanOnLinePayment { get; init; }
}
