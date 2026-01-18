using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IOptionItemQuestionService : IBaseServiceRelation<OptionItemQuestion>
{
    Task<IEnumerable<OptionItemQuestion>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<OptionItemQuestion> adds, IEnumerable<OptionItemQuestion> removes)>
        ChangeQuestionsOfOptionItemAsync(
            long optionItemId,
            IEnumerable<long> questionIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );
}
