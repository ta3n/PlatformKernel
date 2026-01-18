namespace Liberty.Reservation.Site.Public.WebAPI.Models;

public record BookingSearchModel
{
    public long? CheckInDate { get; init; }
    public long? CheckOutDate { get; init; }
    public long? DisplayCheckInDate { get; set; }
    public long? DisplayCheckOutDate { get; set; }
    public int? RestNumber { get; init; }
    public int? RoomNumber { get; init; }
    public int? MinPrice { get; init; }
    public int? MaxPrice { get; init; }
    public string? Secret { get; init; }
    public bool? DayUse { get; init; }
    public IEnumerable<PersonOfBookingSearchModel>? GuestsPerRoom { get; set; }
    public IEnumerable<OptionOfBookingSearchModel>? OptionItems { get; init; }
}

public record PersonOfBookingSearchModel
{
    public long AppDateId { get; init; }
    public int RestIndex { get; init; }
    public int RoomGroupIndex { get; init; }
    public string? PersonAgeTypeCode { get; init; }
    public int? Persons { get; init; }
    public int? MalePersons { get; init; }
    public int? FemalePersons { get; init; }
}

public record OptionOfBookingSearchModel
{
    public long AppDateId { get; init; }
    public int RoomGroupIndex { get; init; }
    public string? OptionItemCode { get; init; }
    public int Number { get; init; }
}
