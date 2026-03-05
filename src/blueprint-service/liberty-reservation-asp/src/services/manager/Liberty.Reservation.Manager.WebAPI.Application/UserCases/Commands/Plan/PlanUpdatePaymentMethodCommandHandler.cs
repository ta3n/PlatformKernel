namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdatePaymentMethodCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService
) : UpdateCommandHandlerBase<PlanUpdatePaymentMethodCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdatePaymentMethodCommand request,
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
