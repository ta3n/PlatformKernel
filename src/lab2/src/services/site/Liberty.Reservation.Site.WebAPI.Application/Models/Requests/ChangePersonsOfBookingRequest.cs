namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record ChangePersonsOfBookingRequest
{
    public required long CheckInDate { get; init; }
    public required int RestNumber { get; init; }
    public required int RoomNumber { get; init; }
    public IEnumerable<PersonOfBookingSearchModel>? PersonOfBooking { get; init; }
    public IEnumerable<OptionOfBookingResultRequest>? OptionsResult { get; init; }

    public long GetCheckOutDate()
    {
        var checkInDate = AppDate.GetDateTime(CheckInDate);
        var checkOutDate = checkInDate.AddDays(RestNumber);

        return AppDate.GetId(checkOutDate);
    }
}

public record OptionOfBookingResultRequest
{
    public long AppDateId { get; init; }
    public int RoomGroupIndex { get; init; }
    public long Id { get; init; }
    public string? Name { get; init; }
    public decimal? Price { get; init; }
    public int? Number { get; init; }
    public decimal? TotalOptionPrice => Price * Number;
}
