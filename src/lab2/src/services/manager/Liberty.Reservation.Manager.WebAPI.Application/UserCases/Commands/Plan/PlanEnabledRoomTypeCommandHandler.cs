using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanEnabledRoomTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IPlanRoomGroupService planRoomGroupService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<PlanEnabledRoomTypeCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanEnabledRoomTypeCommand request,
        CancellationToken cancellationToken
    )
    {
        var planRoomGroup = await planRoomGroupService.FindByPlanIdAndRomTypeIdAsync(
            request.PlanId,
            request.RomTypeId,
            cancellationToken
        );
        planRoomGroup.IsEnabled = request.Payload.IsEnabled;

        var editRoomOfPlan = await planRoomGroupService.UpdateAsync(
            planRoomGroup,
            cancellationToken: cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return editRoomOfPlan.RoomGroupId;
    }
}
