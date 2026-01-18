using System.Text.Json.Serialization;

namespace Liberty.GmoPaymentGateway.Models.Responses;

public class ExecTranResponse : BaseOnLinePaymentResponse
{
    [JsonPropertyName("OrderID")]
    public string? OrderId { get; set; }

    [JsonPropertyName("TranID")]
    public string? TranId { get; set; }

    public string? Forward { get; set; }
    public string? Method { get; set; }
    public string? PayTimes { get; set; }
    public string? Approve { get; set; }
    public string? TranDate { get; set; }
}
