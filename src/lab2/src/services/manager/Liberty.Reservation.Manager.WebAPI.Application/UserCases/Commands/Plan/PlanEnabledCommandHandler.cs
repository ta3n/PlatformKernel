using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanEnabledCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<PlanEnabledCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanEnabledCommand request,
        CancellationToken cancellationToken
    )
    {
        var editPlan = await planService.EnableAsync(
            request.Id,
            request.Payload.IsEnabled,
            true,
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return editPlan.Id;
    }
}
