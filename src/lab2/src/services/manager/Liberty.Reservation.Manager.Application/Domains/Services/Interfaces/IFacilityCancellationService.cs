using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityCancellationService : IBaseServiceRelation<FacilityCancellation>
{
    Task<IEnumerable<FacilityCancellation?>> DeleteFacilityCancellation(
        long cancellationId,
        bool autoSave
    );
}
