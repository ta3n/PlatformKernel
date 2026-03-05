using System.Text.Json.Serialization;

namespace Liberty.GmoPaymentGateway.Models.Responses;

public class OnLinePaymentCancelResponse : BaseOnLinePaymentResponse
{
    [JsonPropertyName("AccessID")]
    public string? AccessId { get; set; }

    [JsonPropertyName("TranID")]
    public string? TranId { get; set; }

    public string? AccessPass { get; set; }
    public string? Forward { get; set; }
    public string? Approve { get; set; }
    public string? TranDate { get; set; }
}
