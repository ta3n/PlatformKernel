using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Models;

public record BookingMetaPersonTypeModel(
    long PlanId,
    long RoomGroupId,
    long SiteId,
    long PersonAgeTypeId,
    MultilingualText? PersonAgeTypeName,
    bool? PersonAgeTypeIsMain,
    int? PersonAgeTypeAgeMin,
    int? PersonAgeTypeAgeMax,
    FoodBeds? PersonAgeTypeFoodBed,
    bool IsRegardAdult,
    PriceSettingTypes PriceSettingType,
    float? Value,
    bool IsEnabled,
    bool FacilityUseSpaTax,
    long DisplayOrder,
    IEnumerable<SpaTaxDataOfBookingMetaPersonTypeModel> SpaTaxDataList
)
{
    /// <summary>
    /// Calculates the price based on the provided price data and the current settings.
    /// </summary>
    /// <param name="priceData">The price data containing the base price.</param>
    /// <returns>
    /// The calculated price as an <see cref="int"/> if applicable; otherwise, <c>null</c>.
    /// Returns <c>null</c> if the price data is not provided or if the current settings are not enabled.
    /// </returns>
    /// <example>
    /// <code>
    /// var price = model.GetPrice(priceData);
    /// </code>
    /// </example>
    public int? GetPrice(
        PriceData? priceData
    )
    {
        // null は設定がないなど受け入れられない状態
        if (priceData is null)
        {
            return null;
        }

        // 受け入れ
        if (!IsEnabled)
        {
            return null;
        }

        var price = priceData.Price;

        // 大人料金とみなす
        return PersonAgeTypeIsMain ?? false
            ? price
            : GetPrice(price, PriceSettingType, Value);
    }

    /// <summary>
    /// Calculates the price based on the provided booking meta price data and the current settings.
    /// </summary>
    /// <param name="priceData">The booking meta price data containing the base price.</param>
    /// <returns>
    /// The calculated price as an <see cref="int"/> if applicable; otherwise, <c>null</c>.
    /// Returns <c>null</c> if the price data is not provided or if the current settings are not enabled.
    /// </returns>
    /// <example>
    /// <code>
    /// var price = model.GetPrice(bookingMetaPriceData);
    /// </code>
    /// </example>
    public int? GetPrice(
        BookingMetaPriceDataModel? priceData
    )
    {
        // null は設定がないなど受け入れられない状態
        if (priceData is null)
        {
            return null;
        }

        // 受け入れ
        if (!IsEnabled)
        {
            return null;
        }

        var price = priceData.Price;

        // 大人料金とみなす
        return PersonAgeTypeIsMain ?? false
            ? price
            : GetPrice(price, PriceSettingType, Value);
    }

    /// <summary>
    /// Calculates the price based on the provided base price, price setting type, and value.
    /// </summary>
    /// <param name="price">The base price to calculate from.</param>
    /// <param name="priceSettingType">The type of price setting to apply.</param>
    /// <param name="value">The value to use in the calculation.</param>
    /// <returns>
    /// The calculated price as an <see cref="int"/> if applicable; otherwise, <c>null</c>.
    /// </returns>
    /// <example>
    /// <code>
    /// var calculatedPrice = BookingMetaPersonTypeModel.GetPrice(1000, PriceSettingTypes.Percent, 10f);
    /// </code>
    /// </example>
    private static int? GetPrice(
        int? price,
        PriceSettingTypes priceSettingType,
        float? value
    )
    {
        var val = value ?? 0f;
        var p = price ?? 0;

        return priceSettingType switch
        {
            PriceSettingTypes.None => price,
            PriceSettingTypes.Percent => (int)Math.Round(p * val / 100),
            PriceSettingTypes.Price => (int)val,
            PriceSettingTypes.Discount => p - (int)val,
            _ => price
        };
    }

    /// <summary>
    /// Calculates the spa tax based on the provided price and the current settings.
    /// </summary>
    /// <param name="price">The price to calculate the spa tax for.</param>
    /// <returns>
    /// The calculated spa tax as an <see cref="int"/> if applicable; otherwise, <c>null</c>.
    /// Returns <c>0</c> if the facility does not use spa tax.
    /// </returns>
    /// <example>
    /// <code>
    /// var spaTax = model.GetSpaTax(1500f);
    /// </code>
    /// </example>
    public int? GetSpaTax(
        float? price
    )
    {
        if (price is null)
        {
            return null;
        }

        if (!FacilityUseSpaTax)
        {
            return 0;
        }

        var tax = SpaTaxDataList
            .Where(x => x.PriceMin is not null && x.PriceMin <= price)
            .Where(x => x.PriceMax is not null && x.PriceMax >= price)
            .Select(x => x.Tax)
            .FirstOrDefault();

        return tax;
    }
}

public record SpaTaxDataOfBookingMetaPersonTypeModel(
    long SpaTaxDataId,
    int? PriceMax,
    int? PriceMin,
    int? Tax,
    bool IsEnabled
);
