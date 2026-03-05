using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

public record BookingByPlanResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Url { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? Phone { get; set; }
    public long? CreatedAt { get; set; }
    public string? Description { get; set; }

    public List<BookingDataByPlanResponse>? BookingData { get; set; }
}

public record BookingDataByPlanResponse(
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
    string? Description
)
{
    public decimal? MinTotalPrice { get; set; }
    public decimal? BasePrice { get; set; }
    public TimeSpan? CheckInEnd { get; set; }
    public List<FileOfPlanResponse>? Files { get; set; }
    public List<MealOfPlanResponse>? Meals { get; set; }
    public List<RoomOfPlanResponse> Rooms { get; set; } = [];
}

public record FileOfPlanResponse(
    string? Code,
    string? ContentType,
    int Index,
    bool IsEnabled
);

public record RoomOfPlanResponse(
    long Id,
    string? Name,
    string? Tag,
    bool IsEnabledSmoking,
    string? Overview,
    long DisplayOrder
)
{
    public List<FileOfPlanResponse>? Files { get; init; }
    public List<AppDatePriceOfPlanResponse> AppDatePrices { get; set; } = [];
}

public record MealOfPlanResponse(
    long Id,
    MealTypeEatTypes MealTypeEatType,
    string? Name
);

public record AppDatePriceOfPlanResponse(
    long AppDateId,
    int? RemainNumber
)
{
    public decimal? BasePrice { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalPrice => Price + TotalSpaTax;

    [property: JsonIgnore]
    public long RoomGroupId { get; set; }
}
