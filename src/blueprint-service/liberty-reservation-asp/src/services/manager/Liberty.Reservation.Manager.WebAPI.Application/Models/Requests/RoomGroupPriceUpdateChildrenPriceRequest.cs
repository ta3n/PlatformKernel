namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupPriceUpdateChildrenPriceRequest(
    List<RoomTypeUpdateChildrenPersonAgeTypeRequest>? PersonAgeTypes
);

public record RoomTypeUpdateChildrenPersonAgeTypeRequest(
    long PersonAgeTypeId,
    bool IsEnabled,
    bool IsRegardAdult,
    PriceSettingTypes PriceSettingType,
    float? Value
);
