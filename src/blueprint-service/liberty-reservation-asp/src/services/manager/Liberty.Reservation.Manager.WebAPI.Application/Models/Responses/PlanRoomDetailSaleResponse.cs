namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomDetailSaleResponse
{
    public long PlanId { get; init; }
    public long RomTypeId { get; init; }
    public long SiteId { get; init; }
    public int? AutoExtendEveryMonthDay { get; init; }
    public int? AutoExtendMonth { get; init; }
    public bool UseAutoExtend { get; init; }
}
