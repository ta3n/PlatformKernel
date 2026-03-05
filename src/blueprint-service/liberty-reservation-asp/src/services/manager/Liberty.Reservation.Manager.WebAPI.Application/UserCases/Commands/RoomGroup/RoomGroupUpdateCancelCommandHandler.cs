namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdateCancelCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IPlanRoomGroupService planRoomGroupService
) : UpdateCommandHandlerBase<RoomGroupUpdateCancelCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupUpdateCancelCommand request,
        CancellationToken cancellationToken
    )
    {
        var planRoomGroup = await planRoomGroupService.FindByRoomGroupIdAsync(
            request.Id,
            cancellationToken
        );

        var response = await mediator.Send(
            new PlanUpdateCancelCommand(planRoomGroup.PlanId) { Payload = request.Payload },
            cancellationToken
        );

        return response;
    }
}
