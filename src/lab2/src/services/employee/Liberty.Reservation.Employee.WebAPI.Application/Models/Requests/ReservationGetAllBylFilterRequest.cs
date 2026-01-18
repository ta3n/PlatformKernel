namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record ReservationGetAllBylFilterRequest(
    string? FacilitySearch,
    string? SiteSearch,
    string? RoomSearch,
    string? PlanSearch,
    string? Code,
    string? Name,
    string? Kana,
    string? PostCode,
    string? Address1,
    string? PhoneNumber,
    string? EMail,
    string? FreeInput,
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
    IEnumerable<PaymentTypes>? PaymentTypes
);
