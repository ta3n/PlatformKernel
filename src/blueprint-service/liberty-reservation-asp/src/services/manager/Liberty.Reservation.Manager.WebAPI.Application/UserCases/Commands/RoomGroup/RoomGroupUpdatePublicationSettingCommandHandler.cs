using Liberty.Reservation.Manager.Application.Auth;
using PlanEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Plan;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdatePublicationSettingCommandHandler(
    ILogger<RoomGroupUpdateDisplaySettingCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ISiteService siteService,
    IRoomGroupService roomGroupService,
    IPlanService planService,
    IPlanSiteService planSiteService
) : UpdateCommandHandlerBase<RoomGroupUpdatePublicationSettingCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupUpdatePublicationSettingCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var existingCountSite = await siteService.CountByIdsAsync(
            payload.SiteIds ?? [],
            cancellationToken
        );
        if (existingCountSite != (payload.SiteIds?.Length ?? 0))
        {
            throw new SiteNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var editRoomGroup = await roomGroupService.FindByIdAsync(
                payload.Id ?? 0,
                cancellationToken
            );
            await UpdatePlanAsync(payload, facilityId, editRoomGroup, true, cancellationToken);

            _ = await roomGroupService.ChangeSitesOfRoomGroupAsync(
                editRoomGroup.Id,
                payload.SiteIds ?? [],
                false,
                cancellationToken
            );

            await roomGroupService.UpdateLastModifiedAsync(editRoomGroup.Id, cancellationToken);

            await UnitOfWork.CommitAsync(cancellationToken);

            return editRoomGroup.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update display of room group failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task UpdatePlanAsync(
        RoomGroupUpdatePublicationSettingRequest payload,
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
            payload.Id ?? 0,
            facilityId,
            cancellationToken
        );

        if (planForRoomOnly is null)
        {
            planForRoomOnly = await planService.CreatePlanWithRoomOnlyTypeAsync(
                editRoomGroup.Name!.GetValueByHeader(),
                facilityId,
                false,
                cancellationToken
            );

            _ = await planService.CreatePlanWithRoomOnlyTypeRelationAsync(
                editRoomGroup.Id,
                planForRoomOnly,
                false,
                cancellationToken
            );
        }

        _ = await planSiteService.ChangeSitesOfPlanAsync(
            planForRoomOnly.Id,
            payload.SiteIds?.ToList() ?? [],
            false,
            cancellationToken
        );

        var requestUpdatePlan = Mapper.Map<RoomGroupUpdatePublicationSettingRequest, PlanEntity>(payload);
        planForRoomOnly = UpdatePlan(planForRoomOnly, requestUpdatePlan);

        _ = await planService.UpdateAsync(
            planForRoomOnly,
            false,
            cancellationToken: cancellationToken
        );
    }

    private static PlanEntity UpdatePlan(
        PlanEntity planToUpdate,
        PlanEntity updatePlan
    )
    {
        planToUpdate.UseDisplayDate = updatePlan.UseDisplayDate;
        planToUpdate.DisplayDateStart = updatePlan.DisplayDateStart;
        planToUpdate.DisplayDateEnd = updatePlan.DisplayDateEnd;
        planToUpdate.UseAcceptDate = updatePlan.UseAcceptDate;
        planToUpdate.AcceptDateStart = updatePlan.AcceptDateStart;
        planToUpdate.AcceptDateEnd = updatePlan.AcceptDateEnd;
        planToUpdate.AcceptDays = updatePlan.AcceptDays;
        planToUpdate.AcceptMonths = updatePlan.AcceptMonths;
        planToUpdate.AcceptEndLimitType = updatePlan.AcceptEndLimitType;
        planToUpdate.ReceptionDayLimit = updatePlan.ReceptionDayLimit;
        planToUpdate.ReceptionLimit = updatePlan.ReceptionLimit;

        return planToUpdate;
    }
}
