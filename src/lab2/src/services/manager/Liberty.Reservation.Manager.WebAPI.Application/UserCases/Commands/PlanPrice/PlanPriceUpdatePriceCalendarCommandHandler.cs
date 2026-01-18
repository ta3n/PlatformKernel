using Liberty.Cache.Services;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceUpdatePriceCalendarCommandHandler(
    ILogger<PlanPriceUpdatePriceCalendarCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IDapperUnitOfWork dapperUnitOfWork,
    IMapper mapper,
    IPlanRoomGroupSiteAppDateService planRoomGroupSiteAppDateService,
    IPlanRoomGroupSiteAppDatePriceDataService planRoomGroupSiteAppDatePriceDataService,
    IPlanService planService,
    IFacilityService facilityService,
    IPlanRoomGroupSiteService planRoomGroupSiteService,
    ISecurityContextAccessor securityContextAccessor,
    ICacheService cacheService
) : UpdateCommandHandlerBase<PlanPriceUpdatePriceCalendarCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanPriceUpdatePriceCalendarCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        if (payload.PriceDatas is null || payload.PriceDatas.Count == 0) return request.PlanId;

        var (startDate, endDate) = GetDateRange(payload.PriceDatas!);
        var keyLock = $"lock:{request.PlanId}-{request.RoomTypeId}-{request.SiteId}-{startDate}-{endDate}:";
        var lockValue = await cacheService.GetAsync<(long, long)>(keyLock, cancellationToken);
        if (Overlaps(lockValue.Item1, lockValue.Item2, startDate, endDate))
        {
            throw new InvalidOperationException($"Range [{startDate}-{endDate}] is processing.");
        }
        await cacheService.SetAsync(keyLock, (startDate, endDate), cancellationToken);
        try
        {
            await CheckValidationMinimumPriceAsync(request, CancellationToken.None);

            var (priceDataOfSiteInPlanRoom, appDatesOfSiteInPlanRoom) = AdjustPriceDataOfSiteInPlanRoom(request);
            try
            {
                await dapperUnitOfWork.BeginTransactionAsync(cancellationToken: CancellationToken.None);

                _ = await planRoomGroupSiteAppDatePriceDataService.ChangePriceDataOfSiteInPlanRoom(
                    request.PlanId,
                    request.RoomTypeId,
                    request.SiteId,
                    priceDataOfSiteInPlanRoom,
                    false,
                    true,
                    dapperUnitOfWork.Transaction ?? throw new InvalidOperationException("Transaction not started"),
                    CancellationToken.None
                );

                _ = await planRoomGroupSiteAppDateService.ChangeDateDataOfSiteInPlanRoom(
                    request.PlanId,
                    request.RoomTypeId,
                    request.SiteId,
                    appDatesOfSiteInPlanRoom,
                    false,
                    true,
                    dapperUnitOfWork.Transaction ?? throw new InvalidOperationException("Transaction not started"),
                    CancellationToken.None
                );

                await dapperUnitOfWork.CommitAsync(CancellationToken.None);

                await planService.UpdateLastModifiedAsync(request.PlanId, CancellationToken.None);
                return request.PlanId;
            }
            catch (Exception ex)
            {
                await dapperUnitOfWork.RollbackAsync(CancellationToken.None);
                throw new AppLibertyException(ex.Message, ex);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {ErrorMessage}",
                nameof(PlanPriceUpdatePriceCalendarCommandHandler),
                ex.Message
            );
            throw new AppLibertyException(ex.Message, ex);
        }
        finally
        {
            await cacheService.RemoveAsync(keyLock, cancellationToken);
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

        var hashing = new HashSet<long>();

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
                        IsEnabled = true,
                        Code =
                            $"{request.PlanId}{request.RoomTypeId}{request.SiteId}{priceData.DateCalendar}{priceData.PersonMin}{priceData.PersonMax}{priceData.Price}"
                    },
                    IsEnabled = true
                }
            );
            if (hashing.Add(priceData.DateCalendar))
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

    private static (long startDate, long endDate) GetDateRange(
        List<RomTypeUpdatePriceDataCalendarRequest> input
    )
    {
        var start = input.Min(a => a.DateCalendar);
        var end = input.Max(a => a.DateCalendar);
        return (start, end);
    }

    private static bool Overlaps(
        long aStart,
        long aEnd,
        long bStart,
        long bEnd
    )
        => aStart <= bEnd && aEnd >= bStart;
}
