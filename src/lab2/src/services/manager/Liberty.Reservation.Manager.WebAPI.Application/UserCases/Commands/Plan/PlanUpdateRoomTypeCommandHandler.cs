using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateRoomTypeCommandHandler(
    ILogger<PlanUpdateRoomTypeCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IPlanService planService,
    IPlanRoomGroupService planRoomGroupService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<PlanUpdateRoomTypeCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateRoomTypeCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingPlan = await planService.FindByIdWithIncludeAsync(
            request.Id,
            cancellationToken
        );

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await planRoomGroupService.ChangePlanRoomGroupAsync(
                existingPlan.Id,
                request.Payload.RoomTypeIds!,
                false,
                cancellationToken
            );

            var editPlan = await planService.UpdateAsync(existingPlan, false, cancellationToken: cancellationToken);

            await UnitOfWork.CommitAsync(cancellationToken);

            var facilityId = securityContextAccessor.FacilityKey;

            await cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
                false,
                cancellationToken
            );

            return editPlan.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanUpdateRoomTypeCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
