using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models;

public record BookingMetaDiscountDataModel(
    long PlanId,
    long RoomGroupId,
    long SiteId,
    long DiscountDataId,
    int? StartPrevDay,
    int? EndPrevDay,
    int? PersonMin,
    int? PersonMax,
    float? Value,
    PriceSettingTypes PriceSettingType
)
{
    /// <summary>
    /// Determines whether the specified previous day is within the range defined by the start and end previous days.
    /// </summary>
    /// <param name="prevDay">The previous day to check.</param>
    /// <returns>
    /// <c>true</c> if the previous day is within the range; otherwise, <c>false</c>.
    /// </returns>
    public bool InPrevDay(
        int prevDay
    )
    {
        var startPrevDay = StartPrevDay;
        var endPrevDay = EndPrevDay;

        if (startPrevDay > prevDay)
        {
            return false;
        }

        return !(endPrevDay < prevDay);
    }

    /// <summary>
    /// Determines whether the specified number of persons is within the range defined by the minimum and maximum persons.
    /// </summary>
    /// <param name="persons">The number of persons to check.</param>
    /// <returns>
    /// <c>true</c> if the number of persons is within the range; otherwise, <c>false</c>.
    /// </returns>
    public bool InRange(
        int? persons
    )
    {
        if (PersonMin > persons)
        {
            return false;
        }

        return !(PersonMax < persons);
    }

    /// <summary>
    /// Calculates the discount based on the specified price and the discount value.
    /// </summary>
    /// <param name="price">The original price to apply the discount to.</param>
    /// <returns>
    /// The calculated discount as an <see cref="int"/> if applicable; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// The discount calculation depends on the <see cref="PriceSettingType"/>:
    /// <list type="bullet">
    /// <item><description><c>None</c>: Returns 0.</description></item>
    /// <item><description><c>Percent</c>: Applies the discount as a percentage of the price.</description></item>
    /// <item><description><c>Price</c>: Returns the discount value directly.</description></item>
    /// <item><description><c>Discount</c>: Returns <c>null</c>.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var discount = model.GetDiscount(1000);
    /// </code>
    /// </example>
    public int? GetDiscount(
        int? price
    )
    {
        if (Value is null)
        {
            return null;
        }

        if (price is null)
        {
            return null;
        }

        switch (PriceSettingType)
        {
            case PriceSettingTypes.None:
                return 0;
            case PriceSettingTypes.Percent:
                var p = price;
                var val2 = p * Value / 100;
                return (int)Math.Round((double)val2);
            case PriceSettingTypes.Price:
                return (int)Value;
            case PriceSettingTypes.Discount:
            default:
                return null;
        }
    }
}
