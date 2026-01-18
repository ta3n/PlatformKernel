namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateMealCommandHandler(
    ILogger<PlanUpdateMealCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    IPlanMealTypeService planMealTypeService,
    IMealTypeService mealTypeService
) : UpdateCommandHandlerBase<PlanUpdateMealCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateMealCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var editMealTypesOfPlan = payload.MealTypes;

        var existingPlan = await planService.FindByIdWithIncludeAsync(
            request.Id,
            cancellationToken
        );

        if (editMealTypesOfPlan is { Count: > 0 })
        {
            var mealTypeIds = editMealTypesOfPlan.Select(f => f.Id).Distinct().ToArray();
            var existingCount = await mealTypeService.CountByIdsAsync(
                mealTypeIds,
                cancellationToken
            );

            if (existingCount != mealTypeIds.Length)
            {
                throw new MealTypeNotfoundException();
            }
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            List<PlanMealType> mealTypesOfPlan = [];
            if (editMealTypesOfPlan is { Count: > 0 })
            {
                mealTypesOfPlan.AddRange(
                    request.Payload.MealTypes!.Select(
                        meal => new PlanMealType
                        {
                            Plan = existingPlan,
                            MealTypeId = meal.Id,
                            MealTypeEatType = meal.MealTypeEatType
                        }
                    )
                );
            }

            _ = await planMealTypeService.ChangePlanMealTypeAsync(
                existingPlan.Id,
                mealTypesOfPlan,
                false,
                cancellationToken
            );

            await planService.UpdateLastModifiedAsync(
                existingPlan.Id,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return request.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanUpdateMealCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
