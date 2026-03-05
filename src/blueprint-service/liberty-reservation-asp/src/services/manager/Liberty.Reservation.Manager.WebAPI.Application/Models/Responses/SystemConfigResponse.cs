namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record SystemConfigResponse(
    long Id,
    string Code,
    bool? CanOnlinePayment
);
