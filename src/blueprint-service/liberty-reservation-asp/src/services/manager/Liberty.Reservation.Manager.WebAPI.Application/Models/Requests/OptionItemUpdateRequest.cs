using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record OptionItemUpdateRequest(
    string Name,
    string Description,
    [property: JsonRequired] int BaseNumber,
    [property: JsonRequired] int Price,
    long[]? OptionCategoryIds,
    long[]? MasterCategoryIds,
    long[]? QuestionIds,
    ImageOfOptionItemUpdateRequest[]? Images
)
{
    public long? Id { get; set; }
}

public record ImageOfOptionItemUpdateRequest(
    long Id,
    int Index
);
