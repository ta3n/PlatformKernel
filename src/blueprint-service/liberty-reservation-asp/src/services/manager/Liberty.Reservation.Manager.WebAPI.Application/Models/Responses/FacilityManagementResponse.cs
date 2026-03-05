namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record FacilityManagementResponse(
    string? Code,
    string? RecordCode,
    string? Name,
    bool IsEnabled
);
