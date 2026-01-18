namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record AdjustOptionsBookingRequest
{
    public required long CheckInDate { get; init; }
    public required int RestNumber { get; init; }
    public required int RoomNumber { get; init; }
    public IEnumerable<PersonOfBookingResultRequest>? PersonResult { get; init; }
    public IEnumerable<OptionOfBookingSearchModel>? OptionOfBooking { get; init; }
}

public record PersonOfBookingResultRequest(
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
)
{
    public long AppDateId { get; init; }
    public int RestIndex { get; init; }
    public int RoomGroupIndex { get; init; }
}

public record OptionOfBookingSearchModel
{
    public long AppDateId { get; init; }
    public int RoomGroupIndex { get; init; }
    public long OptionItemId { get; init; }
    public int Number { get; init; }
}
