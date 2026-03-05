using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Plan;

public class PlanUpdateQuestionCommandHandler(
    ILogger<PlanUpdateQuestionCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanService planService,
    IQuestionService questionService,
    IPlanQuestionService planQuestionService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor
) : UpdateCommandHandlerBase<PlanUpdateQuestionCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanUpdateQuestionCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingPlan = await planService.FindByIdWithIncludeAsync(
            request.Id,
            cancellationToken
        );

        var existingQuestionsCount = await questionService.CountByIdsAsync(
            payload.QuestionIds?.ToArray() ?? [],
            cancellationToken
        );
        if (existingQuestionsCount != payload.QuestionIds?.Count)
        {
            throw new QuestionNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await planQuestionService.ChangeQuestionsOfPlanAsync(
                existingPlan.Id,
                payload.QuestionIds ?? [],
                false,
                cancellationToken
            );

            await planService.UpdateAsync(
                existingPlan,
                false,
                cancellationToken: cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            var facilityId = securityContextAccessor.FacilityKey;

            await cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternQuestionByFacilityId, facilityId),
                false,
                cancellationToken
            );

            return existingPlan.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PlanUpdateQuestionCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
