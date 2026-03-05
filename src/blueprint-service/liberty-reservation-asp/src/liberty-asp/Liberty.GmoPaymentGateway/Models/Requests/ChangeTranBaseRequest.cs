using Newtonsoft.Json;

namespace Liberty.GmoPaymentGateway.Models.Requests;

public record ChangeTranBaseRequest(
    string OrderId,
    string AccessId,
    [property: JsonIgnore] string AccessPass,
    string Amount,
    string Tax,
    string Method,
    string PayTimes
);
