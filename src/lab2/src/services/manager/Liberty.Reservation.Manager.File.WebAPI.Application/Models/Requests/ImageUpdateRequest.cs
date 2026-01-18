using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;

public record ImageUpdateRequest(
    [property: JsonRequired] bool IsEnabled,
    int? Index,
    string? Description,
    string[]? Tags,
    List<FilePurposeTypes>? FilePurposeTypes,
    List<long>? ImageCategoryIds,
    List<long>? MasterCategoryIds
)
{
    public long? Id { get; set; }
}
