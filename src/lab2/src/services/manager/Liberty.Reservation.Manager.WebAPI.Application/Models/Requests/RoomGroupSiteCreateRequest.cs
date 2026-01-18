namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupSiteCreateRequest(
    long PlanId,
    long RomTypeId,
    long SiteId
);
