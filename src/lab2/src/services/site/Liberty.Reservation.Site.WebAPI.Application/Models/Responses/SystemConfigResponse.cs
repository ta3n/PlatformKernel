namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record SystemConfigResponse(
    long Id,
    string Code,
    bool? CanOnlinePayment
);
