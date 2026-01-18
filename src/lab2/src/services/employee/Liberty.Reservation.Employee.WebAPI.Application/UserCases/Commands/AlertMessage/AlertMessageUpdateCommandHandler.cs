using Liberty.Cache.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.AlertMessage;

public class AlertMessageUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IAlertMessageService alertMessageService
) : UpdateCommandHandlerBase<AlertMessageUpdateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        AlertMessageUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingAlertMessage = await alertMessageService.ExistingAlertMessageAsync(
            payload.Id ?? 0,
            cancellationToken
        );

        if (!existingAlertMessage)
        {
            throw new AlertMessageNotFoundException();
        }

        var alertMessage = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.AlertMessage>(payload);

        await alertMessageService.UpdateAsync(alertMessage, cancellationToken: cancellationToken);

        var keyCache = $"{CacheKeys.AllAlertMessageActivePrefixKey}";
        await cacheService.RemoveByPatternsAsync(true, $"*{keyCache}*");

        return alertMessage.Id;
    }
}
