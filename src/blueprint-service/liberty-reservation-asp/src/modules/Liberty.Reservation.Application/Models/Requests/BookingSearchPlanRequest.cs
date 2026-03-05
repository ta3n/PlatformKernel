using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Responses;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Models.Requests;

public record BookingSearchPlanRequest : BookingSearchModel
{
    [JsonIgnore]
    public bool UseCache { get; init; } = true;

    public BookingSearchPlanRequest()
    {
        CheckInDate = AppDate.GetId(DateTime.Today);
    }

    public long[] GetOptionItemIds()
    {
        var listOptionItems = OptionItems is not null ? OptionItems!.ToList() : [];
        if (listOptionItems is not { Count: > 0 })
        {
            return [];
        }

        return
        [
            .. listOptionItems.Select(x => x.OptionItemId).Distinct()
        ];
    }
}
