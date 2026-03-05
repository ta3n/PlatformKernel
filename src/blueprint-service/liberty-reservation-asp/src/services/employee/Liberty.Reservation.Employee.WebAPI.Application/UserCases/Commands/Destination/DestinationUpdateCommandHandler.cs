using Liberty.Cache.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;

public class DestinationUpdateCommandHandler(
    ILogger<DestinationUpdateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    ISiteService siteService
) : UpdateCommandHandlerBase<DestinationUpdateCommand, DestinationResponse>(unitOfWork, mapper)
{
    protected override async Task<DestinationResponse> HandleAsync(
        DestinationUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var destination = Mapper.Map<Site>(payload);

        var checkExistingCode = await siteService.CheckExistingCode(
            payload.Code,
            [payload.Id ?? 0],
            cancellationToken
        );
        if (checkExistingCode)
        {
            throw new SiteDuplicatedCodeException(payload.Code);
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var editDestination = await siteService.UpdateAsync(
                destination,
                false,
                cancellationToken: cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            var response = Mapper.Map<DestinationResponse>(editDestination);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update destination failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    protected override void RemoveCaches(
        DestinationUpdateCommand request
    )
    {
        cacheService.RemoveByPatterns(
            true,
            CacheKeys.ResetPatternManagerFacility
        );
    }
}
