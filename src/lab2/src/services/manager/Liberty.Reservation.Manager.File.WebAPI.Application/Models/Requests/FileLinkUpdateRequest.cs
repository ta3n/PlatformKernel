using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;

public record FileLinkUpdateRequest(
    [property: JsonRequired] int Index
)
{
    [JsonIgnore]
    public string? FileCode { get; set; }

    [JsonIgnore]
    public string? RecordCode { get; set; }
}
