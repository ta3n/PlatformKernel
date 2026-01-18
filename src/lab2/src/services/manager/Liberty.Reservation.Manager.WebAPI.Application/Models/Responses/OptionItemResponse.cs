namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record OptionItemResponse(
    long Id,
    string? Name,
    string? Description,
    int? BaseNumber,
    int? Price,
    bool IsEnabled,
    bool IsUseInPlan,
    IEnumerable<ImageOfOptionItemResponse>? Images
);

public record ImageOfOptionItemResponse(
    long Id,
    string Code,
    int Index,
    bool IsEnabled,
    string? Description
);
