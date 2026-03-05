using System.Text.Json.Serialization;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Models.Requests;

public record BookingPriceRequest
{
    public required long CheckInDate { get; init; }
    public required int RestNumber { get; init; }
    public required int RoomNumber { get; init; }
    public required List<PersonOfBookingPriceRequest> GuestsPerRoom { get; init; }
    public List<OptionOfBookingPriceRequest>? OptionItems { get; init; }
    public string? Secret { get; init; }

    [JsonIgnore]
    public long ReservationId { get; set; } = 0;

    /// <summary>
    /// Calculates the end date of the stay based on the check-in date and the number of nights stayed.
    /// </summary>
    /// <returns>
    /// The end date of the stay as a unique long identifier, calculated using the application's date system.
    /// </returns>
    /// <example>
    /// <code>
    /// var endDate = bookingPriceRequest.GetDateEndNight();
    /// </code>
    /// </example>
    public long GetDateEndNight()
    {
        var dateEndNight = AppDate.GetId(
            AppDate.GetDateTime(CheckInDate).AddDays(RestNumber - 1)
        );

        return dateEndNight;
    }

    /// <summary>
    /// Retrieves a distinct array of option item IDs from the booking request.
    /// </summary>
    /// <returns>
    /// An array of unique option item IDs as <see cref="long"/> if available; otherwise, <c>null</c>.
    /// </returns>
    /// <example>
    /// <code>
    /// var optionItems = bookingPriceRequest.GetOptionItems();
    /// </code>
    /// </example>
    public long[]? GetOptionItems()
    {
        return OptionItems?.Select(x => x.OptionItemId).Distinct().ToArray();
    }

    [JsonIgnore]
    public bool ExecThrowException { get; set; } = true;
}

public record PersonOfBookingPriceRequest
{
    public long AppDateId { get; init; }
    public int RestIndex { get; init; }

    public int RoomGroupIndex { get; init; }
    public long PersonAgeTypeId { get; init; }
    public int? Persons { get; init; }

    public int? MalePersons { get; init; }
    public int? FemalePersons { get; init; }
}

public record OptionOfBookingPriceRequest
{
    public long AppDateId { get; init; }
    public int RoomGroupIndex { get; init; }
    public long OptionItemId { get; init; }
    public int Number { get; init; }
}
