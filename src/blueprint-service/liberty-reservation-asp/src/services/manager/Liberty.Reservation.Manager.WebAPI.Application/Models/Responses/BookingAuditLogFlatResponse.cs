namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record BookingAuditLogFlatResponse(
    DateTime? ChangeDate,
    string? ChangedBy,
    string? ChangeItem,
    object? ChangeItemData,
    object? BeforeValue,
    object? AfterValue
);
