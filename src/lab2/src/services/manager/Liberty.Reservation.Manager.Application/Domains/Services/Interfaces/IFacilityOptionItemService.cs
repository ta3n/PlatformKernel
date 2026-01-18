using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityOptionItemService : IBaseServiceRelation<FacilityOptionItem>
{
    Task<IEnumerable<FacilityOptionItem>> FindAllByOptionItemIdAsync(
        long optionItemId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<FacilityOptionItem>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
