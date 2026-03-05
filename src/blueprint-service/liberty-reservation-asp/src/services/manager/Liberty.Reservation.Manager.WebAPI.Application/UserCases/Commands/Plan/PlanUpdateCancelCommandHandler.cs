namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateCancelCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService
) : UpdateCommandHandlerBase<PlanUpdateCancelCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateCancelCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingPlan = await planService.FindByIdAsync(
            request.Id,
            cancellationToken
        );

        Mapper.Map(request.Payload, existingPlan);

        var editPlan = await planService.UpdateAsync(
            existingPlan,
            cancellationToken: cancellationToken
        );

        return editPlan.Id;
    }
}
