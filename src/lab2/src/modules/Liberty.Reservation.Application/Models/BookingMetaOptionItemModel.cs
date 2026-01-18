using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models;

public record BookingMetaOptionItemModel(
    long PlanId,
    long OptionItemId,
    int? Number,
    OptionItemTargets OptionItemTarget,
    PlanOptionItemTypes PlanOptionItemType,
    IEnumerable<BookingMetaPlanOptionItemAppDate> AppDates
);

public record BookingMetaPlanOptionItemAppDate(
    long OptionItemId,
    MultilingualText? OptionItemName,
    int? OptionItemPrice,
    long AppDateId,
    bool IsNotSelled,
    int? SellNumber,
    int? MaxSupplyNumber,
    int ReservedNumber,
    IEnumerable<BookingMetaReservationOptionItemAppDate> ReservationOptionItems
)
{
    public int? RemainNumber => (SellNumber ?? 0) - ReservedNumber;
}

public record BookingMetaReservationOptionItemAppDate(
    long ReservationId,
    long RoomGroupId,
    long OptionItemId,
    long BookingDateId,
    int RestIndex,
    int RoomGroupIndex,
    decimal Price,
    int Number,
    decimal TotalPrice
);
