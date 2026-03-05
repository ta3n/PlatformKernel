namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomDetailDiscountResponse(
    long PlanId,
    long RomTypeId,
    long SiteId,
    List<RoomTypeDiscountDataResponse>? DiscountDatas
);

public record RoomTypeDiscountDataResponse
{
    public int? StartPrevDay { get; init; }
    public int? EndPrevDay { get; init; }
    public int? PersonMin { get; init; }
    public int? PersonMax { get; init; }
    public float? Value { get; init; }
    public PriceSettingTypes PriceSettingType { get; init; }
}
