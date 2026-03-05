using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.SystemConfig;

public class SystemConfigCanOnlinePaymentCommandHandler(
    ILogger<SystemConfigCanOnlinePaymentCommand> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISystemConfigRepository systemConfigRepository,
    ISystemConfigService systemConfigService
) : UpdateCommandHandlerBase<SystemConfigCanOnlinePaymentCommand, SystemConfigResponse>(unitOfWork, mapper)
{
    protected override async Task<SystemConfigResponse> HandleAsync(
        SystemConfigCanOnlinePaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var entitySystemConfig = await systemConfigRepository.GetQueryableWithAsNoTracking().SingleOrDefaultAsync(cancellationToken)
            ?? throw new SystemConfigNotfoundException();
        try
        {
            entitySystemConfig.CanOnlinePayment = payload.CanOnlinePayment;
            await systemConfigService.UpdateCanOnlinePaymentOfSystemConfigAsync(
                entitySystemConfig,
                cancellationToken
            );

            var response = Mapper.Map<SystemConfigResponse>(entitySystemConfig);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update system config failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
