using Liberty.Reservation.Application.Constants;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Models.Responses;

public record BookingSearchByPlanResponse(
    long Id,
    string? Name,
    string? Tag,
    bool IsOnLinePayment,
    bool IsOnSidePayment,
    string? Summary,
    bool UseDisplayDate,
    long? DisplayDateStart,
    long? DisplayDateEnd,
    bool UseAcceptDate,
    long? AcceptDateStart,
    long? AcceptDateEnd,
    PlanTypes PlanType,
    long DisplayOrder,
    string? Description,
    bool DayUse
)
{
    public long? FacilityId { get; set; }
    public decimal? MinTotalPrice { get; set; }
    public decimal? BasePrice { get; set; }
    public TimeSpan? CheckInStart { get; set; }
    public TimeSpan? CheckInEnd { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public string? Code { get; set; }
    public List<CategoryOfPlanResponse>? Categories { get; set; }
    public List<FileOfPlanResponse>? Files { get; set; }
    public List<MealOfPlanResponse>? Meals { get; set; }
    public List<RoomOfPlanResponse> Rooms { get; set; } = [];
}

public record AppDatePriceOfPlanResponse(
    long AppDateId,
    int? RemainNumber
)
{
    public AppDatePriceStatusSearchModel Status { get; set; } = new();
    public decimal? BasePrice { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalPrice => Price + TotalSpaTax;

    [property: JsonIgnore]
    public long RoomGroupId { get; set; }
}

public record CategoryOfPlanResponse(
    long Id,
    string? Name
)
{
    public string? Code { get; set; }
}

public record FileOfPlanResponse(
    string? Code,
    string? ContentType,
    int Index,
    bool IsEnabled
);

public record MealOfPlanResponse(
    long Id,
    MealTypeEatTypes MealTypeEatType,
    string? Name
)
{
    public string? Code { get; set; }
}

public record RoomOfPlanResponse(
    long Id,
    string? Name,
    string? Tag,
    bool IsEnabledSmoking,
    string? Overview,
    long DisplayOrder
)
{
    public string? Code { get; set; }
    public List<FileOfPlanResponse>? Files { get; init; }
    public List<AppDatePriceOfPlanResponse> AppDatePrices { get; set; } = [];
}

public record PriceCalendarOfRoomResponse
{
    public decimal? BasePrice => AppDatePrices
        .Where(x => x.Status is { IsDayBookable: true, IsNight: true })
        .Select(x => x.BasePrice)
        .Min();

    public TimeSpan? CheckInEnd { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? MinTotalPrice { get; set; }
    public List<AppDatePriceOfPlanResponse> AppDatePrices { get; set; } = [];
}
