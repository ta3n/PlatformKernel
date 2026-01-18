namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomDetailChildrenPriceResponse(
    long PlanId,
    long RomTypeId,
    long SiteId,
    List<RoomTypeChildrenPersonAgeTypeResponse>? PersonAgeTypes
);

public record RoomTypeChildrenPersonAgeTypeResponse
{
    public long? PersonAgeTypeId { get; init; }
    public string? Name { get; init; }
    public bool? IsEnabled { get; init; }
    public bool? IsRegardAdult { get; init; }
    public PriceSettingTypes PriceSettingType { get; init; }
    public float? Value { get; init; }
    public bool? IsMain { get; init; }
    public long DisplayOrder { get; init; }
}
