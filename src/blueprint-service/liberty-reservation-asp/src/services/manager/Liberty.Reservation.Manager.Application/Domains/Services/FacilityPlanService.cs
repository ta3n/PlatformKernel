namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityPlanService(
    ILogger<FacilityPlanService> logger,
    IFacilityPlanRepository repository
) : BaseServiceRelation<FacilityPlan>(logger, repository), IFacilityPlanService
{
    public async Task<IEnumerable<string>> GetFacilityCodeByPlanCodAsync(
        string planCode,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = repository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Plan!.Code == planCode)
            .Select(x => x.Facility!.Code!);

        return await queryable.ToListAsync(cancellationToken);
    }
}
