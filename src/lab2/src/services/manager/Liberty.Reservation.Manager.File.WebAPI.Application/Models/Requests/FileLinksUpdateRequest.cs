using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;

public record FileLinksUpdateRequest(
    IEnumerable<FileOfLinkUpdateRequest> Files
)
{
    [JsonIgnore]
    public string? RecordCode { get; set; }
}

public record FileOfLinkUpdateRequest(
    [property: JsonRequired] string? FileCode,
    [property: JsonRequired] int Index,
    [property: JsonRequired] FileLinkStates States
);

public enum FileLinkStates
{
    Adjust,
    Delete
}
