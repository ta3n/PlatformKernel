namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

public class RoomGroupUpdatePaymentMethodCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IPlanRoomGroupService planRoomGroupService
) : UpdateCommandHandlerBase<RoomGroupUpdatePaymentMethodCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        RoomGroupUpdatePaymentMethodCommand request,
        CancellationToken cancellationToken
    )
    {
        var planRoomGroup = await planRoomGroupService.FindByRoomGroupIdAsync(
            request.Id,
            cancellationToken
        );
        var response = await mediator.Send(
            new PlanUpdatePaymentMethodCommand(planRoomGroup.PlanId) { Payload = request.Payload },
            cancellationToken
        );

        return response;
    }
}
