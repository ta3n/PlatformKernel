using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

public class CancellationPolicyDeleteCommandHandler(
    ILogger<CancellationPolicyDeleteCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IServiceProvider serviceProvider,
    ISecurityContextAccessor securityContextAccessor
) : DeleteCommandHandlerBase<CancellationPolicyDeleteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        CancellationPolicyDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        var cancellationService = serviceProvider.GetRequiredService<ICancellationService>();
        var facilityCancellationService = serviceProvider.GetRequiredService<IFacilityCancellationService>();
        var cancellationDataService = serviceProvider.GetRequiredService<ICancellationDataService>();
        var dataOfCancellationService = serviceProvider.GetRequiredService<IDataOfCancellationService>();
        var planService = serviceProvider.GetRequiredService<IPlanService>();

        var payload = request.Payload;

        var cancellationCount = await cancellationService.CountByIdsAsync(
            [payload.Id],
            cancellationToken
        );
        if (cancellationCount == 0)
        {
            throw new CancellationNotfoundException();
        }

        var isHasAnyPlanUsingCancellations = await planService.IsAnyPlanEnabledUsingCancellationAsync([payload.Id], cancellationToken);

        if (isHasAnyPlanUsingCancellations)
        {
            throw new CancallationDeleteWhenHasPlanUsingException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var existingDataOfCancellation = (await dataOfCancellationService.FindAllByCancellationIdAsync(
                request.Payload.Id,
                cancellationToken
            )).ToList();
            var cancellationDataIds = existingDataOfCancellation.Select(x => x.CancellationDataId).ToArray();

            if (existingDataOfCancellation is { Count: > 0 })
            {
                await dataOfCancellationService.DeleteRangeAsync(
                    existingDataOfCancellation,
                    false,
                    cancellationToken
                );
                await cancellationDataService.DeleteRangeAsync(
                    cancellationDataIds,
                    false,
                    cancellationToken
                );
            }

            var removeCancellation = await cancellationService.DeleteAsync(
                payload.Id,
                false,
                cancellationToken
            );
            await facilityCancellationService.DeleteFacilityCancellation(
                request.Payload.Id,
                false
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            var facilityId = securityContextAccessor.FacilityKey;

            await cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
                false,
                cancellationToken
            );

            return removeCancellation.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete cancellation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
