using System.Text.Json.Serialization;

namespace Liberty.GmoPaymentGateway.Models.Responses;

public class OnLinePaymentSearchTradeResponse : BaseOnLinePaymentResponse
{
    [JsonPropertyName("OrderID")]
    public string? OrderId { get; set; }

    public string? Status { get; set; }
    public string? ProcessDate { get; set; }
    public string? JobCd { get; set; }

    [JsonPropertyName("AccessID")]
    public string? AccessId { get; set; }

    public string? AccessPass { get; set; }
    public string? ItemCode { get; set; }
    public string? Amount { get; set; }
    public string? Tax { get; set; }
    public string? SiteId { get; set; }
    public string? MemberId { get; set; }
    public string? CardNo { get; set; }
    public string? Expire { get; set; }
    public string? Method { get; set; }
    public string? PayTimes { get; set; }
    public string? Forward { get; set; }

    [JsonPropertyName("TranID")]
    public string? TranId { get; set; }

    public string? Approve { get; set; }
    public string? ClientField1 { get; set; }
    public string? ClientField2 { get; set; }
    public string? ClientField3 { get; set; }
}
