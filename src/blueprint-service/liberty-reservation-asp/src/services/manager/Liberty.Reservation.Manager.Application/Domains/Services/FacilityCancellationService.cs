namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityCancellationService(
    ILogger<FacilityCancellationService> logger,
    IFacilityCancellationRepository facilityCancellationRepository
) : BaseServiceRelation<FacilityCancellation>(logger, facilityCancellationRepository), IFacilityCancellationService
{
    public async Task<IEnumerable<FacilityCancellation?>> DeleteFacilityCancellation(
        long cancellationId,
        bool autoSave
    )
    {
        var facilityCancellation = facilityCancellationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.CancellationId == cancellationId)
            .AsEnumerable();

        await facilityCancellationRepository.DeleteRangeAsync(facilityCancellation, autoSave);

        return facilityCancellation;
    }
}
