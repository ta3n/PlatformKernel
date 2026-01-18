namespace Liberty.Reservation.Application.Models;

public record BookingAuditLogModel(
    string? EventType,
    long AggregateId,
    string? AggregateCode,
    string? Request,
    long FacilityId,
    long SiteId,
    string? UserCode,
    long? ExistReserveId = null,
    string? LanguageCode = "ja"
);
