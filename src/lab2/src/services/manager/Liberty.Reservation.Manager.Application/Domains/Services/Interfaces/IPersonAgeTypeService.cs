using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPersonAgeTypeService : IBaseService<PersonAgeType>
{
    Task<IPage<PersonAgeType>> GetAllPersonAgeTypes(
        IPageable pageable,
        bool isVisible = false,
        CancellationToken cancellationToken = default
    );
}
