using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceUpdateChildrenPriceCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanRoomGroupSitePersonAgeTypeService planRoomGroupSite,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IPlanService planService
) : UpdateCommandHandlerBase<PlanPriceUpdateChildrenPriceCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanPriceUpdateChildrenPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        var personAgeTypesOfSiteInPlanRoom = await planRoomGroupSite.FindAllByPlanIdAndRomTypeIdAsync(
            request.PlanId,
            request.RoomTypeId,
            request.SiteId,
            cancellationToken
        );

        foreach (var personAgeType in request.Payload.PersonAgeTypes!)
        {
            var planRoomGroupSitePersonAgeType = personAgeTypesOfSiteInPlanRoom.Find(
                x => x.PersonAgeTypeId == personAgeType.PersonAgeTypeId
            );

            if (planRoomGroupSitePersonAgeType is null)
            {
                continue;
            }

            planRoomGroupSitePersonAgeType.IsEnabled = personAgeType.IsEnabled;
            planRoomGroupSitePersonAgeType.IsRegardAdult = personAgeType.IsRegardAdult;
            planRoomGroupSitePersonAgeType.PriceSettingType = personAgeType.PriceSettingType;
            planRoomGroupSitePersonAgeType.Value = personAgeType.Value;
        }

        await planRoomGroupSite.UpdateRangeAsync(
            personAgeTypesOfSiteInPlanRoom,
            cancellationToken: cancellationToken
        );

        await planService.UpdateLastModifiedAsync(request.PlanId, cancellationToken);

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return request.PlanId;
    }
}
