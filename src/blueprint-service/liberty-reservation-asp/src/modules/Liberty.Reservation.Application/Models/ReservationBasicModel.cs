using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Models;

public record ReservationBasicModel(
    long Id,
    string? Code,
    string? UserCode,
    long FacilityId,
    long SiteId,
    long PlanId,
    long RoomGroupId,
    long CheckInDate,
    int RestNumber,
    int RoomNumber,
    string? LanguageCode,
    bool IsUnConfirmed,
    ReservationStatus Status,
    List<OptionOfBookingSearchModel> OptionOfBookingSearchModel
);

public record ReservationRoomGroupAppDateOptionItemModel(
    long OptionItemId,
    long BookingDateId,
    int RestIndex,
    int RoomGroupIndex,
    int Number
);

public record GroupedReservationOptionItemModel(
    long OptionItemId,
    long AppDateId,
    int TotalNumber
);
