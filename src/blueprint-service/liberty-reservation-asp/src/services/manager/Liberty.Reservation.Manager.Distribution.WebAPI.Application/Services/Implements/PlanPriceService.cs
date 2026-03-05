using System.Collections.Frozen;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services.Implements;

public class PlanPriceService(
    IPlanRoomGroupRepository planRoomGroupRepository,
    IPlanRoomGroupSiteAppDatePriceDataRepository planRoomGroupSiteAppDatePriceDataRepository,
    IRoomGroupAppDateRepository roomGroupAppDateRepository
) : IPlanPriceService
{
    public async Task<IEnumerable<PlanRoomGroupDto>> GetPlanPriceAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = planRoomGroupRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.Plan!.FacilityPlans!.Any(a => a.Facility!.Code == facilityCode))
            .Where(x => x.RoomGroup!.GroupName != null)
            .Select(
                x => new PlanRoomGroupDto
                {
                    ScAgtPlanCode = x.Plan!.Code,
                    ScAgtRoomCode = x.RoomGroup!.GroupName,
                    PlanIndicationName = x.Plan!.Name!.GetValueByHeader(),
                    EnforcementPeriodFrom = x.Plan!.BookingReceptionStart.ToString() ?? string.Empty,
                    EnforcementPeriodTo = x.Plan!.BookingReceptionEnd.ToString() ?? string.Empty,
                    ReserveInformationEndDay = x.Plan!.ReceptionDayLimit ?? 0,
                    ReserveInformationEndTime = x.Plan!.ReceptionLimit!.Value.ToString("hhmm"),
                    DisplayOrder = x.Plan!.DisplayOrder
                }
            )
            .OrderByDescending(x => x.DisplayOrder);

        var planRoomGroupDto = await queryable.ToListAsync(cancellationToken);

        return planRoomGroupDto;
    }

    public async Task<IEnumerable<TariffData>> GetPlanPriceTariffDataAsync(
        string planCode,
        string siteCode,
        string roomGroupName,
        long fromDateSearch,
        long toDateSearch,
        CancellationToken cancellationToken = default
    )
    {
        var appDateMap = roomGroupAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.AppDateId >= fromDateSearch && x.AppDateId <= toDateSearch)
            .ToFrozenDictionary(
                x => (x.RoomGroupId, x.AppDateId),
                x => x.IsNotSelled
            );

        var queryable = planRoomGroupSiteAppDatePriceDataRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Plan!.Code == planCode)
            .Where(x => x.Site!.Code == siteCode)
            .Where(x => x.RoomGroup!.GroupName == roomGroupName)
            .Where(x => x.DateCalendar >= fromDateSearch && x.DateCalendar <= toDateSearch)
            .OrderBy(x => x.DateCalendar)
            .ThenBy(x => x.PriceData!.PersonMin)
            .GroupBy(x => x.DateCalendar)
            .Select(
                x => new TariffData
                {
                    Prices = new List<PriceDataPlanRoom>
                    {
                        new()
                        {
                            Date = x.Key,
                            SaleStopState
                                = (int)GetSalesStatusPriceData(x.First().RoomGroupId, x.Key, appDateMap),
                            PriceElement = x.Select(
                                    b => new UpdatePriceDataItem
                                    {
                                        PersonMax = b.PriceData!.PersonMax.ToString(),
                                        PersonMin = b.PriceData!.PersonMin.ToString(),
                                        Price = b.PriceData!.Price.ToString()
                                    }
                                )
                                .ToList()
                        }
                    }
                }
            );

        var planPrices = await queryable.ToListAsync(cancellationToken);

        return planPrices;
    }

    public UpdatePriceDataResponse HandlerDataUpdatePriceDataResponse(
        List<PlanRoomGroupSiteAppDatePriceData> appDatePriceDatas,
        UpdatePlanRoomRequest updatePlanRoomRequest
    )
    {
        var updatePriceDataResponse = new UpdatePriceDataResponse
        {
            ScAgtPlanCode = updatePlanRoomRequest.ScAgtPlanCode,
            ScAgtRoomCode = updatePlanRoomRequest.ScAgtRoomCode,
            ScAgtSiteCode = updatePlanRoomRequest.ScAgtSiteCode,
            PriceData = appDatePriceDatas
                .GroupBy(x => x.DateCalendar)
                .Select(
                    dateGroup =>
                    {
                        var appointedDate = dateGroup.Key;

                        var saleStopState = int.Parse(
                            updatePlanRoomRequest.PriceData?.Find(p => p.AppointedDate == appointedDate.ToString())
                                ?.StopStartDivision!
                        );

                        var priceElements = dateGroup
                            .Where(p => p.PriceData != null)
                            .Select(
                                p => new UpdatePriceDataItem
                                {
                                    PersonMin = p.PriceData!.PersonMin.ToString(),
                                    PersonMax = p.PriceData!.PersonMax.ToString(),
                                    Price = p.PriceData!.Price.ToString()
                                }
                            )
                            .OrderBy(e => int.Parse(e.PersonMin!))
                            .ToList();

                        return new PriceDatum
                        {
                            AppointedDate = appointedDate,
                            SaleStopState = saleStopState,
                            PriceElement = priceElements
                        };
                    }
                )
                .OrderBy(p => p.AppointedDate)
                .ToList()
        };
        return updatePriceDataResponse;
    }

    private static SalesStatusPriceData GetSalesStatusPriceData(
        long roomGroupId,
        long appDate,
        FrozenDictionary<(long RoomGroupId, long AppDateId), bool> appDateMap
    )
    {
        var key = (roomGroupId, appDate);
        var active = appDateMap.TryGetValue(key, out var isNotSelled) ? isNotSelled : (bool?)null;

        return active switch
        {
            true => SalesStatusPriceData.NotSale,
            _ => SalesStatusPriceData.OnSale
        };
    }
}
