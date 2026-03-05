namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateSaleCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService
) : UpdateCommandHandlerBase<PlanUpdateSaleCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateSaleCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingPlan = await planService.FindByIdWithIncludeAsync(
            request.Id,
            cancellationToken
        );

        if (existingPlan.DayUse && request.Payload.NumberOfStayLimitMax is not 1 && request.Payload.NumberOfStayLimitMin is not 1)
        {
            throw new PlanUseDayInvalidNightLimitException();
        }

        Mapper.Map(request.Payload, existingPlan);

        var editPlan = await planService.UpdateAsync(
            existingPlan,
            cancellationToken: cancellationToken
        );

        return editPlan.Id;
    }
}
