using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Int64;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Plan;

public class UpdatePriceDataPlanRoomCommandHandler(
    ILogger<UpdatePriceDataPlanRoomCommandHandler> logger,
    ICacheService cacheService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IServiceProvider serviceProvider
) : UpdateCommandHandlerBase<UpdatePriceDataPlanRoomCommand, BaseDataResponse<UpdatePriceDataResponse>>(unitOfWork, mapper)
{
    private readonly IPlanRoomGroupSiteAppDateService _planRoomGroupSiteAppDateService
        = serviceProvider.GetRequiredService<IPlanRoomGroupSiteAppDateService>();

    private readonly IPlanRoomGroupSiteAppDatePriceDataService _planRoomGroupSiteAppDatePriceDataService
        = serviceProvider.GetRequiredService<IPlanRoomGroupSiteAppDatePriceDataService>();

    private readonly IFacilityService _facilityService
        = serviceProvider.GetRequiredService<IFacilityService>();

    private readonly IPlanRoomGroupSiteService _planRoomGroupSiteService
        = serviceProvider.GetRequiredService<IPlanRoomGroupSiteService>();

    private readonly IRoomGroupService _roomGroupService
        = serviceProvider.GetRequiredService<IRoomGroupService>();

    private readonly ISiteService _siteService
        = serviceProvider.GetRequiredService<ISiteService>();

    private readonly IPlanDistributionService _planDistributionService
        = serviceProvider.GetRequiredService<IPlanDistributionService>();

    private readonly IPlanPriceService _planPriceService
        = serviceProvider.GetRequiredService<IPlanPriceService>();

    private readonly IRoomGroupAppDateService _roomGroupAppDateService
        = serviceProvider.GetRequiredService<IRoomGroupAppDateService>();

    private readonly IAppDateService _appDateService
        = serviceProvider.GetRequiredService<IAppDateService>();

    private readonly IAppDateOfRoomService _appDateOfRoomService
        = serviceProvider.GetRequiredService<IAppDateOfRoomService>();

    private readonly ICheckFacilityService _checkFacilityService
        = serviceProvider.GetRequiredService<ICheckFacilityService>();

    private async Task<bool> IsValidRequest(
        string facilityCode,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey ?? string.Empty;
        var facilityCodesIsValid = await _checkFacilityService.CheckUserManagedFacilityAsync(
            facilityCode,
            userCode,
            cancellationToken
        );

        return facilityCodesIsValid;
    }

    protected override async Task<BaseDataResponse<UpdatePriceDataResponse>> HandleAsync(
        UpdatePriceDataPlanRoomCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityId = await _facilityService.GetFacilityIdByCodeAsync(
            payload.ScAgtFacilityCode ?? string.Empty,
            cancellationToken
        );
        if (facilityId <= 0)
        {
            throw new FieldExternalPlanInvalidException(nameof(UpdatePlanRoomRequest.ScAgtFacilityCode), payload.ScAgtFacilityCode!);
        }

        var isValidFacility = await IsValidRequest(payload.ScAgtFacilityCode!, cancellationToken);
        if (!isValidFacility)
        {
            throw new FacilityForbiddenException();
        }

        var min = payload.PriceData!.Min(p => AppDate.GetDateTime(Parse(p.AppointedDate!)));
        var max = payload.PriceData!.Max(p => AppDate.GetDateTime(Parse(p.AppointedDate!)));
        var now = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
        var isCheckRange356 = ValidDate.BeAValidCheckToDay(now, max, 356);
        if (!isCheckRange356)
        {
            throw new RangeDatePriceDataAfter365InvalidException(now.AddDays(365).ToString("yyyyMMdd"));
        }

        var isValid = ValidDate.BeAValidCheckToDay(min, max, 180);
        if (!isValid)
        {
            throw new RangeDatePriceDataInvalidException(nameof(UpdatePriceData.AppointedDate));
        }

        var roomGroupId = await _roomGroupService.GetRoomGroupByGroupNameAsync(
            payload.ScAgtRoomCode ?? string.Empty,
            cancellationToken
        );

        if (roomGroupId <= 0)
        {
            throw new FieldExternalPlanInvalidException(nameof(UpdatePlanRoomRequest.ScAgtRoomCode), payload.ScAgtRoomCode!);
        }

        var (createAppDatesOfRoomGroups, addAppDates, updateAppDatesOfRoomGroups) =
            await PrepareAppDatesAsync(roomGroupId, payload.PriceData ?? [], cancellationToken);

        var siteId = await _siteService.GetSiteIdBySiteCodeAsync(payload.ScAgtSiteCode, cancellationToken);
        if (siteId <= 0)
        {
            throw new FieldExternalPlanInvalidException(nameof(UpdatePlanRoomRequest.ScAgtSiteCode), payload.ScAgtSiteCode!);
        }

        var planId = await _planDistributionService.GetPlanIdByPlanCode(payload.ScAgtPlanCode, cancellationToken);

        if (planId <= 0)
        {
            throw new FieldExternalPlanInvalidException(nameof(UpdatePlanRoomRequest.ScAgtPlanCode), payload.ScAgtPlanCode!);
        }

        var existingPlanRoomGroupSite = await _planRoomGroupSiteService.FindByPlanIdAndRomTypeIdAsync(
                planId,
                roomGroupId,
                siteId,
                cancellationToken
            )
            ?? throw new PlanRoomGroupSiteNotFoundException(planId, roomGroupId, siteId);

        await CheckValidationMinimumPriceAsync(payload, existingPlanRoomGroupSite, facilityId, cancellationToken);

        var (priceDataOfSiteInPlanRoom, appDatesOfSiteInPlanRoom) = AdjustPriceDataOfSiteInPlanRoom(
            planId,
            siteId,
            roomGroupId,
            payload
        );
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var (addPriceOfSiteInPlanRooms, updatePriceOfSiteInPlanRooms) =
                await _planRoomGroupSiteAppDatePriceDataService.ChangePriceDataOfSiteInPlanRoom(
                    planId,
                    roomGroupId,
                    siteId,
                    priceDataOfSiteInPlanRoom,
                    false,
                    false,
                    null,
                    cancellationToken
                );

            _ = await _planRoomGroupSiteAppDateService.ChangeDateDataOfSiteInPlanRoom(
                planId,
                roomGroupId,
                siteId,
                appDatesOfSiteInPlanRoom,
                false,
                false,
                null,
                cancellationToken
            );

            await _appDateService.CreateRangeAsync(
                addAppDates.Select(
                    x => new AppDate
                    {
                        Id = x.AppDate!.Id,
                        DateTime = x.AppDate!.DateTime,
                        IsEnabled = true
                    }
                ),
                false,
                cancellationToken
            );

            _ = await _roomGroupAppDateService.CreateRangeAsync(
                createAppDatesOfRoomGroups,
                false,
                cancellationToken
            );

            _ = await _roomGroupAppDateService.UpdateRangeAsync(
                updateAppDatesOfRoomGroups,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            var mergedList = addPriceOfSiteInPlanRooms.Concat(updatePriceOfSiteInPlanRooms).ToList();

            var updatePriceData = _planPriceService.HandlerDataUpdatePriceDataResponse(mergedList, payload);

            var response = new BaseDataResponse<UpdatePriceDataResponse> { Data = updatePriceData };

            var compactUserCode = securityContextAccessor.CompactApplicationUserKey;
            await cacheService.RemoveByPatternsAsync(
                true,
                $"*{string.Format(CacheKeys.ExternalGetPlanPricePrefixKey, compactUserCode, string.Empty)}*"
            );

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {ErrorMessage}",
                nameof(UpdatePriceDataPlanRoomCommandHandler),
                ex.Message
            );
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message);
        }
    }

    private async Task CheckValidationMinimumPriceAsync(
        UpdatePlanRoomRequest request,
        PlanRoomGroupSite existingPlanRoomGroupSite,
        long facilityId,
        CancellationToken cancellationToken
    )
    {
        var maxPrice = request.PriceData?
            .SelectMany(x => x.PriceElement ?? [])
            .Where(p => int.TryParse(p.Price, out _))
            .Max(p => int.Parse(p.Price!));
        var minPrice = request.PriceData?
            .SelectMany(x => x.PriceElement ?? [])
            .Where(p => int.TryParse(p.Price, out _))
            .Min(p => int.Parse(p.Price!));
        if (existingPlanRoomGroupSite.IsEnabledMinimumPrice)
        {
            ValidatePriceAgainstMinimum(maxPrice, minPrice, existingPlanRoomGroupSite.MinimumPrice);
            return;
        }

        var facilityMinimumPriceData = await _facilityService.GetMinimumPriceAsync(
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
            long planId,
            long siteId,
            long roomGroupId,
            UpdatePlanRoomRequest payload
        )
    {
        var listPriceDataOfSiteInPlanRoom = new List<PlanRoomGroupSiteAppDatePriceData>();
        var listAppDatesOfSiteInPlanRoom = new List<PlanRoomGroupSiteAppDate>();
        if (payload.PriceData is null)
        {
            return (listPriceDataOfSiteInPlanRoom, listAppDatesOfSiteInPlanRoom);
        }

        foreach (var priceData in payload.PriceData)
        {
            _ = TryParse(priceData.AppointedDate, out var appointedDate);
            foreach (var priceElement in priceData.PriceElement!)
            {
                _ = int.TryParse(priceElement.Price, out var price);
                _ = int.TryParse(priceElement.PersonMin, out var personMin);
                _ = int.TryParse(priceElement.PersonMax, out var personMax);
                listPriceDataOfSiteInPlanRoom.Add(
                    new PlanRoomGroupSiteAppDatePriceData
                    {
                        PlanId = planId,
                        RoomGroupId = roomGroupId,
                        SiteId = siteId,
                        DateCalendar = appointedDate,
                        PriceData = new PriceData
                        {
                            PersonMin = personMin,
                            PersonMax = personMax,
                            Price = price,
                            IsEnabled = true
                        },
                        IsEnabled = true
                    }
                );
            }

            var isExitDateCalendar = listAppDatesOfSiteInPlanRoom.Exists(
                x => x.DateCalendar == appointedDate
            );

            if (!isExitDateCalendar)
            {
                listAppDatesOfSiteInPlanRoom.Add(
                    new PlanRoomGroupSiteAppDate
                    {
                        PlanId = planId,
                        RoomGroupId = roomGroupId,
                        SiteId = siteId,
                        DateCalendar = appointedDate,
                        IsEnabled = true
                    }
                );
            }
        }

        return (listPriceDataOfSiteInPlanRoom, listAppDatesOfSiteInPlanRoom);
    }

    private async Task<(List<RoomGroupAppDate> createList, RoomGroupAppDate[] addAppDates, List<RoomGroupAppDate> updateList)>
        PrepareAppDatesAsync(
            long roomGroupId,
            List<UpdatePriceData> priceData,
            CancellationToken cancellationToken
        )
    {
        var appDateIds = priceData
            .Select(x => Parse(x.AppointedDate!))
            .Distinct()
            .ToArray();

        var appDatesOfRoomGroups = (await _roomGroupAppDateService.FindAllByRoomGroupIdsAsync(
            [roomGroupId],
            appDateIds,
            cancellationToken
        )).ToList();

        var existingAppDates = (await _appDateService.FindAllByIdsAsync(appDateIds, cancellationToken)).ToList();

        var createAppDatesOfRoomGroups = _appDateOfRoomService.CreateAppDatesOfRoomGroups(
            roomGroupId,
            priceData,
            appDatesOfRoomGroups,
            existingAppDates,
            out var updateAppDatesOfRoomGroups
        );

        var addAppDates = createAppDatesOfRoomGroups
            .Where(x => x.AppDate is not null)
            .Select(
                x => new RoomGroupAppDate
                {
                    AppDate = new AppDate
                    {
                        Id = x.AppDate!.Id,
                        DateTime = x.AppDate!.DateTime
                    }
                }
            )
            .Distinct()
            .ToArray();

        createAppDatesOfRoomGroups.ForEach(x => x.AppDate = null);

        return (createAppDatesOfRoomGroups, addAppDates, updateAppDatesOfRoomGroups);
    }
}
