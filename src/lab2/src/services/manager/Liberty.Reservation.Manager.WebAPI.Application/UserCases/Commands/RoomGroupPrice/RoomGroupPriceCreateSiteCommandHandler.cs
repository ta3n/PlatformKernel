using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

public class RoomGroupPriceCreateSiteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupService planRoomGroupService,
    IPlanService planService,
    IRoomGroupService roomGroupService
) : UpdateCommandHandlerBase<RoomGroupPriceCreateSiteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupPriceCreateSiteCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var planForRoomOnly = await planRoomGroupService.GetPlanWithRoomOnlyTypeAsync(
            request.Payload.RomTypeId,
            facilityId,
            cancellationToken
        );

        if (planForRoomOnly is null)
        {
            var roomGroup = await roomGroupService.FindByIdAsync(request.Payload.RomTypeId, cancellationToken);
            planForRoomOnly = await planService.CreatePlanWithRoomOnlyTypeAsync(
                roomGroup.Name!.GetValueByHeader(),
                facilityId,
                cancellationToken: cancellationToken
            );
        }

        var payload = request.Payload with { PlanId = planForRoomOnly.Id };

        var response = await mediator.Send(
            new PlanPriceCreateSiteCommand { Payload = payload },
            cancellationToken
        );

        return response;
    }
}
