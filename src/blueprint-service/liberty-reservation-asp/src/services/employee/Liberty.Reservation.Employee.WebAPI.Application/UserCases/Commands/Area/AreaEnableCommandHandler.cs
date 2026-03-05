using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Area;

public class AresEnableCommandHandler(
    ILogger<AresEnableCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IAreaService areaService,
    IFacilityRepository facilityRepository
) : UpdateCommandHandlerBase<AreaEnableCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        AreaEnableCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
            var areaId = request.Payload.Id ?? 0;

            _ = await areaService.EnableAsync(
                areaId,
                request.Payload.IsEnabled,
                cancellationToken: cancellationToken
            );

            if (!request.Payload.IsEnabled)
            {
                var facilities = await facilityRepository.GetQueryable()
                    .Where(x => x.AreaId == areaId)
                    .ToListAsync(cancellationToken);

                facilities.ForEach(x => x.AreaId = null);

                await facilityRepository.UpdateRangeAsync(facilities, cancellationToken: cancellationToken);
            }

            await UnitOfWork.CommitAsync(cancellationToken);

            return areaId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Enable area failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
