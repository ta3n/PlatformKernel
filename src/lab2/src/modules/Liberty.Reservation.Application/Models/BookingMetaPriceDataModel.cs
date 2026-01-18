namespace Liberty.Reservation.Application.Models;

public record BookingMetaPriceDataModel(
    long PlanId,
    long RoomGroupId,
    long SiteId,
    long AppDateId,
    long PriceDataId,
    int? Price,
    int? PersonMin,
    int? PersonMax
)
{
    /// <summary>
    /// Determines whether the specified number of persons falls within the defined range of minimum and maximum persons.
    /// </summary>
    /// <param name="persons">The number of persons to check.</param>
    /// <returns>
    /// <c>true</c> if the number of persons is within the range; otherwise, <c>false</c>.
    /// Returns <c>false</c> if either the minimum or maximum person limit is not defined.
    /// </returns>
    /// <example>
    /// <code>
    /// var isInRange = bookingMetaPriceDataModel.InRange(5);
    /// </code>
    /// </example>
    public bool InRange(
        int? persons
    )
    {
        var min = PersonMin;
        var max = PersonMax;

        if (min is null)
        {
            return false;
        }

        if (max is null)
        {
            return false;
        }

        if (min > persons)
        {
            return false;
        }

        return !(max < persons);
    }
}
