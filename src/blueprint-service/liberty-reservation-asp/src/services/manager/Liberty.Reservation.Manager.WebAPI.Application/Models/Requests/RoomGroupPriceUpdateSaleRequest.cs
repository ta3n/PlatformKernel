namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupPriceUpdateSaleRequest
{
    public int? AutoExtendEveryMonthDay { get; init; }
    public int? AutoExtendMonth { get; init; }
    public bool? UseAutoExtend { get; init; }
}
