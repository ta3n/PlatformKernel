namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomDetailStandardPriceResponse(
    long PlanId,
    long RomTypeId,
    long SiteId,
    int? CapacityMin,
    int? CapacityMax,
    List<RomTypeStandardPrinceDataResponse>? PriceDatas
);

public record RomTypeStandardPrinceDataResponse
{
    public long DateTypeId { get; init; }
    public int? PersonMin { get; init; }
    public int? PersonMax { get; init; }
    public int? Price { get; init; }
}
