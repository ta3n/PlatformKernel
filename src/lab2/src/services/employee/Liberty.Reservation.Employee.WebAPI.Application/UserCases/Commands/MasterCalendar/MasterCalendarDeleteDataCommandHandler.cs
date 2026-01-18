using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;

public class MasterCalendarDeleteDataCommandHandler(
    ILogger<MasterCalendarDeleteDataCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IAppDateDataService appDateDataService,
    IDateDataOfAppDateService dateDataOfAppDateService
) : DeleteCommandHandlerBase<MasterCalendarDeleteDataCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        MasterCalendarDeleteDataCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingDateDataOfAppDate = await dateDataOfAppDateService.FindByAppDateAsync(
                payload.AppDate,
                cancellationToken
            )
            ?? throw new AppDateDataNotfoundException();
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await dateDataOfAppDateService.DeleteAsync(
                new AppDateAppDateData
                {
                    AppDateId = existingDateDataOfAppDate.AppDateId,
                    AppDateDataId = existingDateDataOfAppDate.AppDateDataId
                },
                false,
                cancellationToken
            );

            await appDateDataService.DeletePhysicalAsync(
                existingDateDataOfAppDate.AppDateData?.Id ?? 0,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);
            await cacheService.ResetAsync($"{nameof(AppDate)}*", cancellationToken: cancellationToken);

            return payload.AppDate;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete app data of app date failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
