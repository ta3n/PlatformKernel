using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilityPlanService : IBaseServiceRelation<FacilityPlan>
{
    Task<IEnumerable<string>> GetFacilityCodeByPlanCodAsync(
        string planCode,
        CancellationToken cancellationToken = default
    );
}
