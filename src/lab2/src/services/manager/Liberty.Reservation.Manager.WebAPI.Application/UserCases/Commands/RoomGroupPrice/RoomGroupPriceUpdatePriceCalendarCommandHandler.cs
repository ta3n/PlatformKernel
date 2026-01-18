using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public class RoomGroupPriceUpdatePriceCalendarCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupService planRoomGroupService,
    IRoomGroupService roomGroupService,
    IPlanService planService
) : UpdateCommandHandlerBase<RoomGroupPriceUpdatePriceCalendarCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupPriceUpdatePriceCalendarCommand request,
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

        var response = await mediator.Send(
            new PlanPrice.PlanPriceUpdatePriceCalendarCommand(
                planForRoomOnly.Id,
                request.RoomTypeId,
                request.SiteId
            ) { Payload = request.Payload },
            cancellationToken
        );

        return response;
    }
}
