namespace Liberty.Reservation.Application.Models.Responses;

public record BookingPriceResponse
{
    public IEnumerable<AppDatePriceOfBookingResponse> AppDatePrices { get; set; } = [];
    public decimal? TotalRoomPrice => AppDatePrices.Sum(x => x.Price);
    public decimal? TotalSpaTax => AppDatePrices.Sum(x => x.TotalSpaTax);
    public decimal? TotalOptionPrice => (int?)AppDatePrices.Sum(x => x.TotalOptionPrice);
    public decimal? TotalPrice => TotalRoomPrice + TotalSpaTax + TotalOptionPrice;
    public bool UseSpaTax { get; set; } = false;
}

public record AppDatePriceOfBookingResponse
{
    public long AppDateId { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalOptionPrice { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalPrice => Price + TotalOptionPrice;
    public List<RoomOfBookingResponse> Rooms { get; set; } = [];
}

public record RoomOfBookingResponse(
    long AppDateId,
    int RoomIndex
)
{
    public decimal Price { get; set; }
    public decimal TotalSpaTax { get; set; }
    public decimal? TotalOptionPrice { get; set; }
    public decimal TotalPrice => Price + TotalOptionPrice ?? 0;
    public List<PeopleOfBookingResponse> Peoples { get; set; } = [];
    public List<OptionOfBookingResponse> Options { get; set; } = [];
}

public record PeopleOfBookingResponse(
    int? Price,
    int? SpaTax,
    int? TotalSpaTax,
    int? TotalPrice,
    int? MalePersons,
    int? FemalePersons,
    int? NonePersons,
    int? Persons,
    int? OtherPersons,
    long PersonAgeTypeId,
    string? Name
);

public record OptionOfBookingResponse(
    long Id,
    string? Name,
    decimal? Price,
    int? Number
)
{
    public decimal? TotalOptionPrice => Price * Number;
}
