using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceUpdatePriceCalendarCommandHandler(
    ILogger<PlanPriceUpdatePriceCalendarCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanRoomGroupSiteAppDateService planRoomGroupSiteAppDateService,
    IPlanRoomGroupSiteAppDatePriceDataService planRoomGroupSiteAppDatePriceDataService,
    IPlanService planService,
    IFacilityService facilityService,
    IPlanRoomGroupSiteService planRoomGroupSiteService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<PlanPriceUpdatePriceCalendarCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanPriceUpdatePriceCalendarCommand request,
        CancellationToken cancellationToken
    )
    {
        await CheckValidationMinimumPriceAsync(request, cancellationToken);

        var (priceDataOfSiteInPlanRoom, appDatesOfSiteInPlanRoom) = AdjustPriceDataOfSiteInPlanRoom(request);
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await planRoomGroupSiteAppDatePriceDataService.ChangePriceDataOfSiteInPlanRoom(
                request.PlanId,
                request.RoomTypeId,
                request.SiteId,
                priceDataOfSiteInPlanRoom,
                false,
                true,
                cancellationToken
            );

            _ = await planRoomGroupSiteAppDateService.ChangeDateDataOfSiteInPlanRoom(
                request.PlanId,
                request.RoomTypeId,
                request.SiteId,
                appDatesOfSiteInPlanRoom,
                false,
                true,
                cancellationToken
            );

            await planService.UpdateLastModifiedAsync(request.PlanId, cancellationToken);

            await UnitOfWork.CommitAsync(cancellationToken);

            return request.PlanId;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {ErrorMessage}",
                nameof(PlanPriceUpdatePriceCalendarCommandHandler),
                ex.Message
            );
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task CheckValidationMinimumPriceAsync(
        PlanPriceUpdatePriceCalendarCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingPlanRoomGroupSite = await planRoomGroupSiteService.FindByPlanIdAndRomTypeIdAsync(
                request.PlanId,
                request.RoomTypeId,
                request.SiteId,
                cancellationToken
            )
            ?? throw new PlanRoomGroupSiteNotFoundException(request.PlanId, request.RoomTypeId, request.SiteId);

        var maxPrice = request.Payload.PriceDatas!.Max(p => p.Price);
        var minPrice = request.Payload.PriceDatas!.Min(p => p.Price);
        if (existingPlanRoomGroupSite.IsEnabledMinimumPrice)
        {
            ValidatePriceAgainstMinimum(maxPrice, minPrice, existingPlanRoomGroupSite.MinimumPrice);
            return;
        }

        var facilityId = securityContextAccessor.FacilityKey;
        var facilityMinimumPriceData = await facilityService.GetMinimumPriceAsync(
                facilityId,
                cancellationToken
            )
            ?? throw new FacilityNotfoundException();

        if (facilityMinimumPriceData.IsEnabledMinimumPrice)
        {
            ValidatePriceAgainstMinimum(maxPrice, minPrice, facilityMinimumPriceData.MinimumPrice);
        }
    }

    private static void ValidatePriceAgainstMinimum(
        int? maxPrice,
        int? minPrice,
        int? minimumPrice
    )
    {
        if (maxPrice < minimumPrice || minPrice < minimumPrice)
        {
            throw new PlanDailyPriceBelowFacilityMinimumException();
        }
    }

    private static (
        List<PlanRoomGroupSiteAppDatePriceData> listPriceDataOfSiteInPlanRoom,
        List<PlanRoomGroupSiteAppDate> listAppDatesOfSiteInPlanRoom
        ) AdjustPriceDataOfSiteInPlanRoom(
            PlanPriceUpdatePriceCalendarCommand request
        )
    {
        var payload = request.Payload;
        var listPriceDataOfSiteInPlanRoom = new List<PlanRoomGroupSiteAppDatePriceData>();
        var listAppDatesOfSiteInPlanRoom = new List<PlanRoomGroupSiteAppDate>();
        if (payload.PriceDatas is null)
        {
            return (listPriceDataOfSiteInPlanRoom, listAppDatesOfSiteInPlanRoom);
        }

        foreach (var priceData in payload.PriceDatas)
        {
            listPriceDataOfSiteInPlanRoom.Add(
                new PlanRoomGroupSiteAppDatePriceData
                {
                    PlanId = request.PlanId,
                    RoomGroupId = request.RoomTypeId,
                    SiteId = request.SiteId,
                    DateCalendar = priceData.DateCalendar,
                    PriceData = new PriceData
                    {
                        PersonMin = priceData.PersonMin,
                        PersonMax = priceData.PersonMax,
                        Price = priceData.Price,
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );

            var isExitDateCalendar = listAppDatesOfSiteInPlanRoom.Exists(
                x => x.DateCalendar == priceData.DateCalendar
            );

            if (!isExitDateCalendar)
            {
                listAppDatesOfSiteInPlanRoom.Add(
                    new PlanRoomGroupSiteAppDate
                    {
                        PlanId = request.PlanId,
                        RoomGroupId = request.RoomTypeId,
                        SiteId = request.SiteId,
                        DateCalendar = priceData.DateCalendar,
                        UseAutoDiscount = priceData.UseAutoDiscount,
                        IsEnabled = true
                    }
                );
            }
        }

        return (listPriceDataOfSiteInPlanRoom, listAppDatesOfSiteInPlanRoom);
    }
}
