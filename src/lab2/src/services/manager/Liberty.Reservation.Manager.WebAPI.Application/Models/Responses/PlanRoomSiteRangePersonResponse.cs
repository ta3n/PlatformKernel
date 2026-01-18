namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomSiteRangePersonResponse(
    long PlanId,
    long RomTypeId,
    long SiteId,
    List<RangePersons>? RangePersons
);

public record RangePersons
{
    public int? PersonMin { get; init; }
    public int? PersonMax { get; init; }
}
