namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public record PlanRoomGroupSiteUpdateMinimumPriceCommand(
    long PlanId,
    long RoomGroupId,
    long SiteId
) : UpdateCommandBase<PlanRoomGroupSiteUpdateMinimumPriceRequest, long>;
