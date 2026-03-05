namespace Liberty.Reservation.Application.Models;

public record BookingMetaDataModel(
    IEnumerable<BookingMetaRoomAppDateModel> RoomAppDates,
    IEnumerable<BookingMetaPlanAppDateModel> PlanAppDates,
    IEnumerable<BookingMetaPersonTypeModel> PersonalTypes,
    IEnumerable<BookingMetaOptionItemModel> OptionItems,
    IEnumerable<BookingMetaPriceDataModel> PriceData,
    IEnumerable<BookingMetaDiscountDataModel> DiscountData,
    IEnumerable<BookingMetaReservationModel> Reservations
);
