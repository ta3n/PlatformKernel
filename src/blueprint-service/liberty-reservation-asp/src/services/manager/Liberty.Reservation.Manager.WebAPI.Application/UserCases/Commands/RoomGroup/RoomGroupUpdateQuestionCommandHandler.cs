namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdateQuestionCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IPlanRoomGroupService planRoomGroupService
) : UpdateCommandHandlerBase<RoomGroupUpdateQuestionCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupUpdateQuestionCommand request,
        CancellationToken cancellationToken
    )
    {
        var planRoomGroup = await planRoomGroupService.FindByRoomGroupIdAsync(
            request.Id,
            cancellationToken
        );

        var response = await mediator.Send(
            new PlanUpdateQuestionCommand(planRoomGroup.PlanId) { Payload = request.Payload },
            cancellationToken
        );

        return response;
    }
}
