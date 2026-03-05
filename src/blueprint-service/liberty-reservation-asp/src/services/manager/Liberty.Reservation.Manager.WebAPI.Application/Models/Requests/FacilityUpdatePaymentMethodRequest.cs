using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdatePaymentMethodRequest(
    [property: JsonRequired] bool IsOnSidePayment,
    [property: JsonRequired] bool IsOnLinePayment,
    string? OnSidePaymentComment,
    string? OnLinePaymentComment,
    string? PaymentComment
);
