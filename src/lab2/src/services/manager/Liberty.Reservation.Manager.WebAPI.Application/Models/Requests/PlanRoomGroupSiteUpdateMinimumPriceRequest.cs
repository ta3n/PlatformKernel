using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanRoomGroupSiteUpdateMinimumPriceRequest(
    [property: JsonRequired] bool IsEnabledMinimumPrice,
    int? MinimumPrice
);
