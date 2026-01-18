using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record SystemConfigCanOnlinePaymentRequest(
    [property: JsonRequired] bool CanOnlinePayment
);
