namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public record PlanPriceUpdateStandardPriceCommand(
    long PlanId,
    long RoomTypeId,
    long SiteId
) : UpdateCommandBase<RoomGroupPriceUpdateStandardPriceRequest, long>;
