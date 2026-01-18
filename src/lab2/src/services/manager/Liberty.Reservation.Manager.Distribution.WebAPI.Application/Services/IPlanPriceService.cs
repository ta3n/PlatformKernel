using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;

public interface IPlanPriceService
{
    Task<IEnumerable<PlanRoomGroupDto>> GetPlanPriceAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<TariffData>> GetPlanPriceTariffDataAsync(
        string planCode,
        string siteCode,
        string roomGroupName,
        long fromDateSearch,
        long toDateSearch,
        CancellationToken cancellationToken = default
    );

    UpdatePriceDataResponse HandlerDataUpdatePriceDataResponse(
        List<PlanRoomGroupSiteAppDatePriceData> appDatePriceDatas,
        UpdatePlanRoomRequest updatePlanRoomRequest
    );
}
