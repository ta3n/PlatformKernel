namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record OptionItemDetailResponse(
    long Id,
    string? Name,
    string? Description,
    int? BaseNumber,
    int? Price,
    IEnumerable<CategoryOfOptionItemDetailResponse>? OptionCategories,
    IEnumerable<CategoryOfOptionItemDetailResponse>? MasterCategories,
    IEnumerable<QuestionOfOptionItemDetailResponse>? Questions,
    IEnumerable<FileOfOptionItemDetailResponse>? Files,
    bool IsEnabled
);

public record CategoryOfOptionItemDetailResponse(
    long Id,
    string? Name
);

public record QuestionOfOptionItemDetailResponse(
    long Id,
    string? Name
);

public record FileOfOptionItemDetailResponse(
    long Id,
    string? Code,
    int Index,
    bool IsEnabled,
    string? Description
);
