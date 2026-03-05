namespace Liberty.GmoPaymentGateway.Models.Responses;

public class BaseOnLinePaymentResponse
{
    public string? ErrCode { get; set; }
    public string? ErrInfo { get; set; }
    public bool HasError => !string.IsNullOrEmpty(ErrCode);
}
