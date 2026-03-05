using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Models;

public class BookingCheckPriceModel
{
    public long SiteId { get; set; }
    public long PlanId { get; set; }
    public long RoomGroupId { get; set; }
    public long CheckInDate { get; init; }
    public int RestNumber { get; init; }
    public int RoomNumber { get; init; }
    public IEnumerable<PersonOfBookingSearchModel>? PersonOfBooking { get; init; }
    public IEnumerable<OptionOfBookingSearchModel>? OptionOfBooking { get; init; }
    public IEnumerable<OptionOfBookingResult>? OptionsResult { get; init; }
    public IEnumerable<PersonOfBookingResult>? PersonResult { get; init; }

    /// <summary>
    /// Calculates the check-out date based on the check-in date and the number of nights stayed.
    /// </summary>
    /// <remarks>
    /// This method uses the `CheckInDate` property to determine the starting date and adds the `RestNumber`
    /// (representing the number of nights) to calculate the check-out date. The result is returned as a unique date ID.
    /// </remarks>
    /// <returns>
    /// The check-out date as a unique long identifier, calculated using the application's date system.
    /// </returns>
    /// <example>
    /// <code>
    /// var checkOutDate = bookingModel.GetCheckOutDate();
    /// </code>
    /// </example>
    public long GetCheckOutDate()
    {
        var checkInDate = AppDate.GetDateTime(CheckInDate);
        var checkOutDate = checkInDate.AddDays(RestNumber);

        return AppDate.GetId(checkOutDate);
    }
}

public record OptionOfBookingResult
{
    public long AppDateId { get; init; }
    public int RoomGroupIndex { get; init; }
    public long Id { get; init; }
    public string? Name { get; init; }
    public decimal? Price { get; init; }
    public int? Number { get; init; }
    public decimal? TotalOptionPrice => Price * Number;
}

public record PersonOfBookingResult(
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
