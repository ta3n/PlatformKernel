using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

public class CancellationPolicyCreateCommandHandler(
    ILogger<CancellationPolicyCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ICancellationService cancellationService,
    IFacilityService facilityService,
    IFacilityCancellationService facilityCancellationService
) : CreateCommandHandlerBase<CancellationPolicyCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        CancellationPolicyCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityId = securityContextAccessor.FacilityKey;
        var count = await facilityService.CountByIdsAsync(
            [facilityId],
            cancellationToken
        );
        if (count == 0)
        {
            throw new FacilityNotfoundException();
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var cancellation = Mapper.Map<Cancellation>(
                payload
            );
            var newCancellation = await cancellationService.CreateAsync(
                cancellation,
                false,
                cancellationToken
            );

            var facilityCancellation = new FacilityCancellation
            {
                FacilityId = facilityId,
                Cancellation = cancellation
            };

            await facilityCancellationService.CreateAsync(
                facilityCancellation,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return newCancellation.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create cancellation failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
