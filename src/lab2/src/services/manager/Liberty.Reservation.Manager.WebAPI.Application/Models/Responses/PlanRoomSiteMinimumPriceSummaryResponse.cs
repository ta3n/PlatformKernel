namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomSiteMinimumPriceSummaryResponse
{
    public PlanRoomSiteMinimumPriceResponse? PlanMinimumPrice { get; init; }
    public FacilityMinimumPriceResponse? FacilityMinimumPrice { get; init; }

    public bool IsEnableMinimumPrice { get; init; }
    public int? MinimumPrice { get; init; }
}
