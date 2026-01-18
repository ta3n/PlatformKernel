namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupPriceUpdateStandardPriceRequest(
    List<RomTypeUpdateStandardRequest>? PriceDatas
);

public record RomTypeUpdateStandardRequest(
    long? DateTypeId,
    int? PersonMin,
    int? PersonMax,
    int? Price
);
