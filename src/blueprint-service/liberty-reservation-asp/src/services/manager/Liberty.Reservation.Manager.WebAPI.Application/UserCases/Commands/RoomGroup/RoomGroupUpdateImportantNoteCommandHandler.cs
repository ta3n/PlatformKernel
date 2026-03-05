namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdateImportantNoteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IPlanRoomGroupService planRoomGroupService
) : UpdateCommandHandlerBase<RoomGroupUpdateImportantNoteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupUpdateImportantNoteCommand request,
        CancellationToken cancellationToken
    )
    {
        var planRoomGroup = await planRoomGroupService.FindByRoomGroupIdAsync(
            request.Id,
            cancellationToken
        );

        var response = await mediator.Send(
            new PlanUpdateImportantNoteCommand(planRoomGroup.PlanId) { Payload = request.Payload },
            cancellationToken
        );

        return response;
    }
}
