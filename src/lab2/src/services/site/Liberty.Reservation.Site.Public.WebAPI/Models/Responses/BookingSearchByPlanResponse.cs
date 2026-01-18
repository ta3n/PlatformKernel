namespace Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

public record BookingSearchByPlanResponse
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Tag { get; set; }
    public bool IsOnLinePayment { get; set; }
    public bool IsOnSidePayment { get; set; }
    public string? Summary { get; set; }
    public bool UseDisplayDate { get; set; }
    public long? DisplayDateStart { get; set; }
    public long? DisplayDateEnd { get; set; }
    public bool UseAcceptDate { get; set; }
    public long? AcceptDateStart { get; set; }
    public long? AcceptDateEnd { get; set; }
    public PlanTypes PlanType { get; set; }
    public long DisplayOrder { get; set; }
    public string? Description { get; set; }
    public bool DayUse { get; set; }
    public string? FacilityCode { get; set; }
    public decimal? MinTotalPrice { get; set; }
    public decimal? BasePrice { get; set; }
    public TimeSpan? CheckInStart { get; set; }
    public TimeSpan? CheckInEnd { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public List<CategoryOfPlanResponse>? Categories { get; set; }
    public List<FileOfPlanResponse>? Files { get; set; }
    public List<MealOfPlanResponse>? Meals { get; set; }
    public List<RoomOfPlanResponse> Rooms { get; set; } = [];
}

public record AppDatePriceOfPlanResponse
{
    public long AppDateId { get; set; }
    public int? RemainNumber { get; set; }
    public AppDatePriceStatusSearchModel Status { get; set; } = new();
    public decimal? BasePrice { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalPrice { get; set; }
}

public record CategoryOfPlanResponse
{
    public string? Code { get; set; }
    public string? Name { get; set; }
}

public record FileOfPlanResponse
{
    public string? Code { get; set; }
    public string? ContentType { get; set; }
    public int Index { get; set; }
    public bool IsEnabled { get; set; }
}

public record MealOfPlanResponse
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public MealTypeEatTypes MealTypeEatType { get; set; }
}

public record RoomOfPlanResponse
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Tag { get; set; }
    public bool IsEnabledSmoking { get; set; }
    public string? Overview { get; set; }
    public long DisplayOrder { get; set; }

    public List<FileOfPlanResponse>? Files { get; init; }
    public List<AppDatePriceOfPlanResponse> AppDatePrices { get; set; } = [];
}

public record PriceCalendarOfRoomResponse
{
    public decimal? BasePrice { get; set; }
    public TimeSpan? CheckInEnd { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? MinTotalPrice { get; set; }
    public List<AppDatePriceOfPlanResponse> AppDatePrices { get; set; } = [];
}
