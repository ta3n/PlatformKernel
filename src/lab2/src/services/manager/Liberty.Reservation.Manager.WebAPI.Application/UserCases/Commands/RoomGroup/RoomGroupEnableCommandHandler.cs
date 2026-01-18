using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupEnableCommandHandler(
    ILogger<RoomGroupEnableCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IRoomGroupService roomGroupService,
    IPlanService planService,
    IPlanRoomGroupRepository planRoomGroupRepository,
    ICacheService cacheService
) : UpdateCommandHandlerBase<RoomGroupEnableCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupEnableCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var editRoomGroup = await roomGroupService.EnableAsync(
                request.Id,
                request.Payload.IsEnabled,
                true,
                cancellationToken
            );

            await UpdatePlanAsync(request, facilityId, editRoomGroup, true, cancellationToken);

            await cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return editRoomGroup.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Enable room group failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task UpdatePlanAsync(
        RoomGroupEnableCommand request,
        long facilityId,
        Reservation.Application.Contexts.DataContexts.Entities.Data.RoomGroup editRoomGroup,
        bool disable,
        CancellationToken cancellationToken
    )
    {
        if (disable)
        {
            return;
        }

        var planForRoomOnly = await planService.GetPlanWithRoomOnlyTypeAsync(
            request.Id,
            facilityId,
            cancellationToken
        );

        if (planForRoomOnly is null)
        {
            _ = await planService.CreatePlanWithRoomOnlyTypeAsync(
                editRoomGroup.Name!.GetValueByHeader(),
                facilityId,
                false,
                cancellationToken
            );
        }

        var planRoom = await planRoomGroupRepository.GetQueryable()
            .Where(x => x.RoomGroupId == request.Id)
            .Where(x => x.Plan!.Equals(planForRoomOnly))
            .SingleOrDefaultAsync(cancellationToken);

        planRoom!.IsEnabled = request.Payload.IsEnabled;

        _ = await planRoomGroupRepository.UpdateAsync(planRoom, cancellationToken: cancellationToken);
    }
}
