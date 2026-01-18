using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceType;

public class PriceTypeCreateCommandHandler(
    ILogger<PriceTypeCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IAppDateTypeService appDateTypeService,
    IFacilityAppDateTypeService facilityAppDateTypeService
) : CreateCommandHandlerBase<PriceTypeCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PriceTypeCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var appDateType = Mapper.Map<AppDateType>(payload);

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newAppDateType = await appDateTypeService.CreateAsync(
                appDateType,
                false,
                cancellationToken
            );

            var newAppDateTypeOfFacility = new FacilityAppDateType
            {
                FacilityId = facilityId,
                AppDateType = newAppDateType,
                IsEnabled = true
            };
            await facilityAppDateTypeService.CreateAsync(
                newAppDateTypeOfFacility,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return newAppDateType.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Handler}: {Message}", nameof(PriceTypeCreateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
