namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdateMealCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IPlanRoomGroupService planRoomGroupService
) : UpdateCommandHandlerBase<RoomGroupUpdateMealCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupUpdateMealCommand request,
        CancellationToken cancellationToken
    )
    {
        var planRoomGroup = await planRoomGroupService.FindByRoomGroupIdAsync(
            request.Id,
            cancellationToken
        );

        var response = await mediator.Send(
            new PlanUpdateMealCommand(planRoomGroup.PlanId) { Payload = request.Payload },
            cancellationToken
        );

        return response;
    }
}
