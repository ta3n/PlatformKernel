namespace Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

public record SystemConfigResponse(
    long Id,
    string Code,
    bool? CanOnlinePayment
);
