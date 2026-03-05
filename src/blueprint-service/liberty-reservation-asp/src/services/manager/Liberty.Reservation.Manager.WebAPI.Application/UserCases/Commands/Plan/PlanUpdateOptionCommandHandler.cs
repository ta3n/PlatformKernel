namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateOptionCommandHandler(
    ILogger<PlanUpdateOptionCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    IOptionItemService optionItemService,
    IPlanOptionItemService planOptionItemService
) : UpdateCommandHandlerBase<PlanUpdateOptionCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateOptionCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var isEditingOptions = (payload.UseFixedOptionItem ?? false) && payload.OptionIds is not null;

        var existingPlan = await planService.FindByIdWithIncludeAsync(
            request.Id,
            cancellationToken
        );

        if (isEditingOptions)
        {
            var existingOptionsCount = await optionItemService.CountByIdsAsync(
                payload.OptionIds?.ToArray() ?? [],
                cancellationToken
            );

            if (existingOptionsCount != payload.OptionIds?.Count)
            {
                throw new OptionItemNotfoundException();
            }
        }

        Mapper.Map(payload, existingPlan);
        existingPlan.UseFixedOptionItem = payload.UseFixedOptionItem ?? false;
        existingPlan.UseOptionalOptionItem = !existingPlan.UseFixedOptionItem;

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            if (isEditingOptions)
            {
                _ = await planOptionItemService.ChangePlanOptionItemAsync(
                    existingPlan.Id,
                    request.Payload.OptionIds ?? [],
                    false,
                    cancellationToken
                );
            }

            var editPlan = await planService.UpdateAsync(
                existingPlan,
                false,
                cancellationToken: cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return editPlan.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanUpdateOptionCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
