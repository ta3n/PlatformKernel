using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateSpecialCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService
) : UpdateCommandHandlerBase<PlanUpdateSpecialCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateSpecialCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingPlan = await planService.FindByIdAsync(
                request.Id,
                cancellationToken
            )
            ?? throw new PlanNotfoundException();

        existingPlan.IsSecret = payload.IsSecret ?? false;
        if (existingPlan.IsSecret)
        {
            existingPlan.SecretWord = payload.SecretWord;
        }

        var editPlan = await planService.UpdateAsync(
            existingPlan,
            cancellationToken: cancellationToken
        );

        return editPlan.Id;
    }
}
