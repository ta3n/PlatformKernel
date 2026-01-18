using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Site.Application.Models.Responses;

public record BookingSearchByRoomResponse(
    long Id,
    string? Name,
    string? Overview,
    long DisplayOrder,
    bool IsEnabledSmoking
)
{
    public decimal? MinTotalPrice { get; set; }
    public List<FileResponse>? Files { get; set; }
    public List<PlanOfRoomSearchResponse>? Plans { get; set; } = [];
}

public record FileResponse(
    string? Code,
    string? ContentType
);

public record PlanOfRoomSearchResponse(
    long Id,
    string? Name,
    PlanTypes PlanType,
    bool DayUse
)
{
    public decimal? BasePrice { get; set; }
    public TimeSpan? CheckInStart { get; set; }
    public TimeSpan? CheckInEnd { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public List<FileResponse>? Files { get; init; }
    public List<AppDatePriceOfPlanResponse> AppDatePrices { get; set; } = [];
}
