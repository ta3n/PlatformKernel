using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanDeleteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor
) : DeleteCommandHandlerBase<PlanDeleteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        var removePlan = await planService.DeleteAsync(
            request.Id,
            cancellationToken: cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return removePlan.Id;
    }
}
