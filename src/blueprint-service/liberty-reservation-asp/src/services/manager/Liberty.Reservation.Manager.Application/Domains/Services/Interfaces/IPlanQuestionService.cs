using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanQuestionService : IBaseServiceRelation<PlanQuestion>
{
    Task<(IEnumerable<PlanQuestion> adds, IEnumerable<PlanQuestion> removes)> ChangeQuestionsOfPlanAsync(
        long planId,
        List<long> planQuestionIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
