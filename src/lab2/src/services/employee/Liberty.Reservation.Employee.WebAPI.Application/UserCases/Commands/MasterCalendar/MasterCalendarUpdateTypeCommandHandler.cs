using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;

public class MasterCalendarUpdateTypeCommandHandler(
    ILogger<MasterCalendarUpdateTypeCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IAppDateService appDateService,
    IAppDateTypeService appDateTypeService,
    IDateTypeOfAppDateService dateTypeOfAppDateService
) : UpdateCommandHandlerBase<MasterCalendarUpdateTypeCommand, DateTypeOfMasterCalendarResponse>(unitOfWork, mapper)
{
    protected override async Task<DateTypeOfMasterCalendarResponse> HandleAsync(
        MasterCalendarUpdateTypeCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingAppDateType = await appDateTypeService.FindByIdAsync(
                payload.AppDateTypeId,
                cancellationToken
            )
            ?? throw new AppDateTypeNotfoundException();
        var existingAppDate = await appDateService.FindByAppDateAsync(
            payload.AppDate,
            cancellationToken
        );

        var existingDateTypesOfAppDate = await dateTypeOfAppDateService.FindAllByAppDateAsync(
            payload.AppDate,
            cancellationToken
        );

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            if (!existingDateTypesOfAppDate.Any())
            {
                var dateTypeOfAppDate = new AppDateAppDateType { AppDateTypeId = payload.AppDateTypeId };
                if (existingAppDate is null)
                {
                    dateTypeOfAppDate.AppDate = new AppDate
                    {
                        Id = payload.AppDate,
                        DateTime = AppDate.GetDateTime(payload.AppDate)
                    };
                }
                else
                {
                    dateTypeOfAppDate.AppDateId = existingAppDate.Id;
                }

                _ = await dateTypeOfAppDateService.CreateAsync(
                    dateTypeOfAppDate,
                    false,
                    cancellationToken
                );
            }
            else
            {
                _ = await dateTypeOfAppDateService.DeleteRangeAsync(
                    existingDateTypesOfAppDate,
                    false,
                    cancellationToken
                );

                var dateTypeOfAppDate = new AppDateAppDateType
                {
                    AppDateId = payload.AppDate,
                    AppDateTypeId = payload.AppDateTypeId
                };

                _ = await dateTypeOfAppDateService.CreateAsync(
                    dateTypeOfAppDate,
                    false,
                    cancellationToken
                );
            }

            await UnitOfWork.CommitAsync(cancellationToken);
            await cacheService.ResetAsync($"{nameof(AppDate)}*", cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update app data of app date failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }

        return new DateTypeOfMasterCalendarResponse(
            payload.AppDate,
            existingAppDateType.Code,
            existingAppDateType.Name,
            existingAppDateType.ShortName,
            existingAppDateType.Color
        );
    }
}
