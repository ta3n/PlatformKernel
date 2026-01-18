using System.Text.Json.Serialization;

namespace Liberty.GmoPaymentGateway.Models.Responses;

public class GetUrlPaymentResponse
{
    public string? LinkUrl { get; set; }
}

public class ErrorGetUrlPaymentResponse
{
    [JsonPropertyName("errCode")]
    public string? ErrCode { get; set; }

    [JsonPropertyName("errInfo")]
    public string? ErrInfo { get; set; }
}
