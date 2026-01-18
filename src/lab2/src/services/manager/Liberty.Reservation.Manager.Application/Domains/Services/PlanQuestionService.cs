using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanQuestionService(
    ILogger<PlanQuestionService> logger,
    IPlanQuestionRepository planQuestionRepository
) : BaseServiceRelation<PlanQuestion>(logger, planQuestionRepository), IPlanQuestionService
{
    public async Task<(IEnumerable<PlanQuestion> adds, IEnumerable<PlanQuestion> removes)>
        ChangeQuestionsOfPlanAsync(
            long planId,
            List<long> planQuestionIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingQuestionsOfPlan = await planQuestionRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);

        var updateQuestionsOfPlan = planQuestionIds
            .Select(
                questionId => new PlanQuestion
                {
                    PlanId = planId,
                    QuestionId = questionId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<PlanQuestion>(
            (
                x,
                y
            ) => x?.QuestionId == y?.QuestionId && x!.PlanId == y?.PlanId,
            obj => obj.PlanId.GetHashCode() ^ obj.QuestionId.GetHashCode()
        );

        var removeQuestionsOfPlan = existingQuestionsOfPlan.Except(
            updateQuestionsOfPlan,
            comparer
        );
        var addQuestionsOfPlan = updateQuestionsOfPlan.Except(
            existingQuestionsOfPlan,
            comparer
        );

        var entitiesToRemove = await DeleteRangeAsync(
            removeQuestionsOfPlan,
            autoSave,
            cancellationToken
        );

        var entitiesToAdd = await CreateRangeAsync(
            addQuestionsOfPlan,
            autoSave,
            cancellationToken
        );
        return (entitiesToAdd, entitiesToRemove);
    }
}
