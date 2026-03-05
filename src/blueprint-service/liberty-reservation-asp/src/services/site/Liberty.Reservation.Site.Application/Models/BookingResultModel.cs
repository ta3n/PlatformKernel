namespace Liberty.Reservation.Site.Application.Models;

public record BookingResultModel
{
    public long Id { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? Tag { get; init; }
    public bool UseAcceptPersonNumber { get; init; }
    public int? AcceptPersonNumberMin { get; init; }
    public int? AcceptPersonNumberMax { get; init; }
    public int? NumberOfStayLimitMin { get; init; }
    public int? NumberOfStayLimitMax { get; init; }
    public bool IsOnLinePayment { get; init; }

    public bool IsOnSidePayment { get; init; }

    // Setting
    public decimal? BasePrice { get; set; }

    // PlanMeta meta
    public string? Description { get; init; }

    // Check Data
    public bool UseDaySaleLimit { get; init; }
    public PlanDaySaleLimitTypes PlanDaySaleLimitType { get; init; }
    public int? RoomNumberDaySaleLimit { get; init; }
    public bool UseDisplayDate { get; init; }
    public long? DisplayDateStart { get; init; }
    public long? DisplayDateEnd { get; init; }
    public bool UseAcceptDate { get; init; }
    public long? AcceptDateStart { get; init; }
    public long? AcceptDateEnd { get; init; }

    public int? GroupNumberDaySaleLimit { get; init; }

    // Relationship
    public long SiteId { get; init; }
    public List<CategoryOfBookingResultModel>? Categories { get; init; }
    public List<FileOfBookingResultModel>? Files { get; init; }
    public List<MealOfBookingResultModel>? Meals { get; init; }
    public required List<RoomOfBookingResultModel> Rooms { get; init; }
    public required List<AppDatePriceOfBookingResultModel> AppDatePrices { get; set; }
}

public record AppDatePriceOfBookingResultModel(
    long AppDateId,
    long PlanId,
    long RoomGroupId,
    long SiteId,
    int? RemainNumber,
    bool IsNotSelled,
    string? Name,
    int? CapacityMax,
    int? CapacityMin,
    int ReservedNumber,
    int ReservationPairs,
    int RoomIndex
)
{
    public decimal? BasePrice { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalOptionPrice { get; set; }
    public decimal? TotalPrice => Price + TotalSpaTax + TotalOptionPrice;
    public List<PeopleOfBookingResultModel> Peoples { get; set; } = [];
    public List<OptionOfBookingResultModel> Options { get; set; } = [];
    public bool IsRest { get; set; }
    public List<long>? NotRestNextDays { get; set; }
}

public record PeopleOfBookingResultModel(
    int? Price,
    int? SpaTax,
    int? TotalSpaTax,
    int? TotalPrice,
    int? MalePersons,
    int? FemalePersons,
    int? NonePersons,
    int? Persons,
    long PersonAgeTypeId,
    string? Name
);

public record CategoryOfBookingResultModel(
    long Id,
    string? Name
);

public record FileOfBookingResultModel(
    string? Code,
    string? ContentType
);

public record MealOfBookingResultModel(
    long Id,
    bool? IsSelected,
    MealTypeEatTypes MealTypeEatType,
    string? Name
);

public record RoomOfBookingResultModel(
    long Id,
    string? Code,
    string? Name,
    bool IsEnabledSmoking,
    string? Tag,
    bool IsUsed
)
{
    public List<FileOfBookingResultModel>? Files { get; init; }
}

public record OptionOfBookingResultModel(
    long Id,
    string? Name,
    decimal? Price,
    int? Number
)
{
    public decimal? TotalOptionPrice => Price * Number;
}
