namespace Liberty.GmoPaymentGateway.Models;

public class GmoErrorItem
{
    public string Code { get; set; } = string.Empty;
    public string DetailCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
