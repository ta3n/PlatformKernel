namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateImportantNoteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService
) : UpdateCommandHandlerBase<PlanUpdateImportantNoteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateImportantNoteCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingPlan = await planService.FindByIdAsync(
            request.Id,
            cancellationToken
        );

        Mapper.Map(request.Payload, existingPlan);

        var editPlan = await planService.UpdateImportantNoteAsync(
            existingPlan,
            cancellationToken: cancellationToken
        );

        return editPlan.Id;
    }
}
