namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupPriceUpdateDiscountRequest(
    List<RomTypeUpdateDiscountDataRequest>? DiscountDatas
);

public record RomTypeUpdateDiscountDataRequest(
    int? StartPrevDay,
    int? EndPrevDay,
    int? PersonMin,
    int? PersonMax,
    PriceSettingTypes PriceSettingType,
    float? Value
);
