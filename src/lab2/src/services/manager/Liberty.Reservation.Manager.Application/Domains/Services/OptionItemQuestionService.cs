using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class OptionItemQuestionService(
    ILogger<OptionItemQuestionService> logger,
    IOptionItemQuestionRepository optionItemQuestionRepository
) : BaseServiceRelation<OptionItemQuestion>(logger, optionItemQuestionRepository), IOptionItemQuestionService
{
    public async Task<IEnumerable<OptionItemQuestion>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    )
    {
        var data = await optionItemQuestionRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.OptionItemId == optionItemId)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<(IEnumerable<OptionItemQuestion> adds, IEnumerable<OptionItemQuestion> removes)>
        ChangeQuestionsOfOptionItemAsync(
            long optionItemId,
            IEnumerable<long> questionIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingQuestionsOfRoomGroup = (await FindAllByOptionItemIdAsync(
            optionItemId,
            cancellationToken
        )).ToList();
        var updateQuestionsOfOptionItem = questionIds
            .Select(
                categoryId => new OptionItemQuestion
                {
                    OptionItemId = optionItemId,
                    QuestionId = categoryId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<OptionItemQuestion>(
            (
                x,
                y
            ) => x?.OptionItemId == y?.OptionItemId && x?.QuestionId == y?.QuestionId,
            obj => obj.OptionItemId.GetHashCode() ^ obj.QuestionId.GetHashCode()
        );

        var removeQuestionsInOptionItem = existingQuestionsOfRoomGroup.Except(
            updateQuestionsOfOptionItem,
            comparer
        );
        var addQuestionsInOptionItem = updateQuestionsOfOptionItem.Except(
            existingQuestionsOfRoomGroup,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removeQuestionsInOptionItem,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addQuestionsInOptionItem,
            autoSave,
            cancellationToken
        );

        return (adds, removes);
    }
}
