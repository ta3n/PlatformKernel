using System.Text.Json.Serialization;

namespace Liberty.Fax.Models.Requests;

public record FaxRequest
{
    [JsonPropertyName("sendto")]
    public IEnumerable<FaxRecipient> SendTo { get; init; } = [];

    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

    [JsonPropertyName("body")]
    public string? Body { get; init; }

    // [JsonPropertyName("userkey")]
    // public string? UserKey { get; init; }
}

public record FaxRecipient
{
    [JsonPropertyName("faxno")]
    public string? FaxNo { get; init; }
}
