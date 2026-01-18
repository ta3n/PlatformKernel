namespace Liberty.GmoPaymentGateway.Models.Requests;

public record CancelOrderRequest(
    string OrderId,
    string AccessId,
    string AccessPass,
    string Amount,
    string Tax,
    string Method,
    string PayTimes
) : ChangeTranBaseRequest(OrderId, AccessId, AccessPass, Amount, Tax, Method, PayTimes)
{
    public bool IsChangeAmount { get; set; }
}
