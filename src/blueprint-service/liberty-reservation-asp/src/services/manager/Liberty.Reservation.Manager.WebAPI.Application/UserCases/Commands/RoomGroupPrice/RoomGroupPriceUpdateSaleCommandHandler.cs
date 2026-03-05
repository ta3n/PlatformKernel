using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public class RoomGroupPriceUpdateSaleCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupService planRoomGroupService,
    IRoomGroupService roomGroupService,
    IPlanService planService
) : UpdateCommandHandlerBase<RoomGroupPriceUpdateSaleCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupPriceUpdateSaleCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var planForRoomOnly = await planRoomGroupService.GetPlanWithRoomOnlyTypeAsync(
            request.RoomTypeId,
            facilityId,
            cancellationToken
        );

        if (planForRoomOnly == null)
        {
            var roomGroup = await roomGroupService.FindByIdAsync(request.RoomTypeId, cancellationToken);
            planForRoomOnly = await planService.CreatePlanWithRoomOnlyTypeAsync(
                roomGroup.Name!.GetValueByHeader(),
                facilityId,
                cancellationToken: cancellationToken
            );
        }

        await roomGroupService.UpdateLastModifiedAsync(
            request.RoomTypeId,
            cancellationToken
        );

        var response = await mediator.Send(
            new PlanPriceUpdateSaleCommand(
                planForRoomOnly.Id,
                request.RoomTypeId,
                request.SiteId
            ) { Payload = request.Payload },
            cancellationToken
        );

        return response;
    }
}
