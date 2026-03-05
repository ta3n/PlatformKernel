using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Models.Responses;

public record BookingSearchModel
{
    public long CheckInDate { get; init; }
    public long CheckOutDate { get; init; }
    public long? DisplayCheckInDate { get; set; }
    public long? DisplayCheckOutDate { get; set; }
    public int RestNumber { get; init; }
    public int RoomNumber { get; init; }
    public int? MinPrice { get; init; }
    public int? MaxPrice { get; init; }
    public string? Secret { get; init; }
    public bool? DayUse { get; init; }
    public IEnumerable<PersonOfBookingSearchModel>? GuestsPerRoom { get; set; }
    public IEnumerable<OptionOfBookingSearchModel>? OptionItems { get; init; }

    /// <summary>
    /// Generates an array of application date IDs between the check-in and check-out dates.
    /// </summary>
    /// <remarks>
    /// This method calculates all the dates between the check-in and check-out dates (inclusive),
    /// converts them to their corresponding application date IDs, and returns them as an array.
    /// </remarks>
    /// <returns>
    /// An array of long values representing the application date IDs for each day
    /// between the check-in and check-out dates.
    /// </returns>
    /// <example>
    /// <code>
    /// var appDates = bookingSearchModel.GetAppDates();
    /// // Example output: [20231001, 20231002, 20231003]
    /// </code>
    /// </example>
    public long[] GetAppDates()
    {
        var appDates = new List<long>();

        var checkIn = AppDate.GetDateTime(CheckInDate);
        var checkOut = AppDate.GetDateTime(CheckOutDate);

        while (checkIn <= checkOut)
        {
            appDates.Add(AppDate.GetId(checkIn));
            checkIn = checkIn.AddDays(1);
        }

        return [.. appDates];
    }
}

public record PersonOfBookingSearchModel
{
    public long AppDateId { get; init; }
    public int RestIndex { get; init; }

    public int RoomGroupIndex { get; init; }
    public long PersonAgeTypeId { get; init; }
    public int? Persons { get; init; }

    public int? MalePersons { get; init; }
    public int? FemalePersons { get; init; }
}

public record OptionOfBookingSearchModel
{
    public long AppDateId { get; init; }
    public int RoomGroupIndex { get; init; }
    public long OptionItemId { get; init; }
    public int Number { get; init; }
}
