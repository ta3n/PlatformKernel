namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record CancellationPolicyDetailResponse(
    long Id,
    string? Name,
    string? Description,
    string? RuleDetail,
    CancellationMeta? Meta,
    IEnumerable<CancellationDataResponse?> CancellationDatas
)
{
    public string? TableSource { get; init; }
};

public record CancellationDataResponse(
    long? Id,
    int DayStart,
    int DayEnd,
    float Rate,
    string? Description
);

public record CancellationMeta(
    string? TableSource
);
