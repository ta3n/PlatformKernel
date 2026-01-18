namespace Liberty.Reservation.Site.Application.Models;

public record BookingPriceResultModel()
{
    public long AppDateId { get; set; }
    public decimal? BasePrice { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalSpaTax { get; set; }
    public decimal? TotalPrice => Price + TotalSpaTax;
    public List<BookingPriceRoomResultModel> Rooms { get; set; } = [];
}

public record BookingPriceRoomResultModel(
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
    public decimal? TotalPrice => Price + TotalSpaTax;
    public List<PeopleOfBookingResultModel> Peoples { get; set; } = [];
    public List<OptionOfBookingResultModel> Options { get; set; } = [];
    public bool IsRest { get; set; }
    public List<long>? NotRestNextDays { get; set; }
}
