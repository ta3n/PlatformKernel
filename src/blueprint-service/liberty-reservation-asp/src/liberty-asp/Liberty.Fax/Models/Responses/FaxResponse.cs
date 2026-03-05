using System.Text.Json.Serialization;

namespace Liberty.Fax.Models.Responses;

public record FaxResponse
{
    [JsonPropertyName("result")]
    public string? Result { get; init; }

    [JsonPropertyName("processkey")]
    public string? ProcessKey { get; init; }

    [JsonPropertyName("accepttime")]
    public string? AcceptTime { get; init; }

    public bool IsSuccess => Result == "000000";
}
