namespace Liberty.Reservation.User.WebAPI.Application.Models.Responses;

public record SystemConfigResponse(
    long Id,
    string Code,
    bool? CanOnlinePayment
);
