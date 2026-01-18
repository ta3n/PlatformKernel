using System.Text.Json.Serialization;

namespace Liberty.GmoPaymentGateway.Models.Responses;

public class EntryTranResponse : BaseOnLinePaymentResponse
{
    [JsonPropertyName("AccessID")]
    public string? AccessId { get; set; }

    public string? AccessPass { get; set; }
}
