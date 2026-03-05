using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityQuestionService : IBaseServiceRelation<FacilityQuestion>
{
    Task EnableAsync(
        long id,
        bool isEnabled,
        CancellationToken cancellationToken = default
    );

    Task<IPage<FacilityQuestion>> FindAllAsync(
        long facilityId,
        IPageable pageable,
        CancellationToken cancellationToken = default
    );

    Task<FacilityQuestion> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<long> CreateFacilityQuestionAsync(
        long facilityId,
        Question question,
        CancellationToken cancellationToken = default
    );

    Task<long> UpdateFacilityQuestionAsync(
        long id,
        Question question,
        CancellationToken cancellationToken = default
    );
}
