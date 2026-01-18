using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;

public record FileUpdateRequest(
    [property: JsonRequired] bool IsEnabled,
    [property: JsonRequired] int Index,
    string? Description,
    string[]? Tags,
    FilePurposeTypes? FilePurposeType,
    List<long>? ImageCategoryIds,
    List<long>? MasterCategoryIds
)
{
    [JsonIgnore]
    public string? Code { get; set; }
}
