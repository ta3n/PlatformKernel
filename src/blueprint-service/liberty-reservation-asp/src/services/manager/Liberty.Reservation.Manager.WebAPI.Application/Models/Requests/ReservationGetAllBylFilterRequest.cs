namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record ReservationGetAllBylFilterRequest(
    string? Code,
    string? Name,
    string? Kana,
    string? PostCode,
    string? Address1,
    string? PhoneNumber,
    string? EMail,
    string? FreeInput,
    string? PlanSearch,
    long? StartCheckInDate,
    long? EndCheckInDate,
    long? StartReservationAcceptanceDate,
    long? EndReservationAcceptanceDate,
    long? StartCancellationDate,
    long? EndCancellationDate,
    long? StartNoShowDate,
    long? EndNoShowDate,
    bool? IsNoShow,
    bool? DayUse,
    IEnumerable<ReservationStatus>? ReservationStatus,
    IEnumerable<long>? SiteIds,
    IEnumerable<PaymentTypes>? PaymentTypes,
    IEnumerable<long>? RoomIds
);
